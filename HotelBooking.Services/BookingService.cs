using HotelBooking.Core.DTOs.Booking;
using HotelBooking.Core.DTOs.Payment;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IGenericRepository<RoomCategory> _categoryRepo;
    private readonly IPaymentService _paymentService;
    private readonly IPromotionService _promotionService;
    private readonly ILoyaltyService _loyaltyService;
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepo,
        IRoomRepository roomRepo,
        IGenericRepository<RoomCategory> categoryRepo,
        IPaymentService paymentService,
        IPromotionService promotionService,
        ILoyaltyService loyaltyService,
        IEmailService emailService,
        ILogger<BookingService> logger)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
        _categoryRepo = categoryRepo;
        _paymentService = paymentService;
        _promotionService = promotionService;
        _loyaltyService = loyaltyService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, string userId)
    {
        // Validate dates
        if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new Core.Exceptions.ValidationException("Check-in date cannot be in the past.");

        if (dto.CheckOutDate <= dto.CheckInDate)
            throw new Core.Exceptions.ValidationException("Check-out date must be after check-in date.");

        // Get room
        var room = await _roomRepo.GetByIdAsync(dto.RoomId);
        if (room == null)
            throw new NotFoundException("Room", dto.RoomId);

        // Check availability
        var isAvailable = await _roomRepo.IsAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (!isAvailable)
            throw new Core.Exceptions.ValidationException("Room is not available for the selected dates.");

        // Get category for pricing
        var category = await _categoryRepo.GetByIdAsync(room.RoomCategoryId);
        if (category == null)
            throw new NotFoundException("RoomCategory", room.RoomCategoryId);

        // Calculate amounts
        var totalNights = (dto.CheckOutDate - dto.CheckInDate).Days;
        var totalAmount = totalNights * category.PricePerNight;
        var discountAmount = 0m;

        // Apply promo code if provided
        if (!string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            discountAmount = await _promotionService.ValidateAndApplyAsync(dto.PromoCode, totalAmount);
        }

        var finalAmount = totalAmount - discountAmount;

        // Generate unique reservation number
        var reservationNumber = await GenerateUniqueReservationNumber();

        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalNights = totalNights,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ReservationNumber = reservationNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepo.AddAsync(booking);

        // Create payment order
        var payment = await _paymentService.CreateOrderAsync(booking.Id, finalAmount);

        var bookingWithDetails = await _bookingRepo.GetWithDetailsAsync(booking.Id);
        var response = MapToResponse(bookingWithDetails!);
        response.Payment = payment;

        return response;
    }

    public async Task<BookingResponseDto> GetBookingAsync(int id, User currentUser)
    {
        var booking = await _bookingRepo.GetWithDetailsAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        // Access control
        if (currentUser.Role == UserRole.User && booking.UserId != currentUser.Id)
            throw new ForbiddenException("You can only view your own bookings.");

        if (currentUser.Role == UserRole.HotelAdmin &&
            booking.Room.RoomCategory.HotelId != currentUser.HotelId)
            throw new ForbiddenException("You can only view bookings for your assigned hotel.");

        return MapToResponse(booking);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(string userId)
    {
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(MapToResponse);
    }

    public async Task<BookingResponseDto> CancelBookingAsync(int id, User currentUser)
    {
        var booking = await _bookingRepo.GetWithDetailsAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        // Access control
        if (currentUser.Role == UserRole.User && booking.UserId != currentUser.Id)
            throw new ForbiddenException("You can only cancel your own bookings.");

        if (currentUser.Role == UserRole.HotelAdmin &&
            booking.Room.RoomCategory.HotelId != currentUser.HotelId)
            throw new ForbiddenException("You can only cancel bookings for your assigned hotel.");

        if (booking.Status != BookingStatus.Confirmed)
            throw new Core.Exceptions.ValidationException("Only confirmed bookings can be cancelled.");

        // Cancellation policy: free if > 24h before check-in, otherwise 10% fee
        var refundAmount = booking.FinalAmount;
        var cancellationFee = 0m;
        var timeToCheckIn = booking.CheckInDate - DateTime.UtcNow;
        if (timeToCheckIn.TotalHours < 24)
        {
            var feePercent = 10;
            if (int.TryParse(Environment.GetEnvironmentVariable("CANCELLATION_FEE_PERCENT"), out var customFee))
            {
                feePercent = customFee;
            }
            cancellationFee = booking.FinalAmount * (feePercent / 100m);
            refundAmount = booking.FinalAmount - cancellationFee;
        }

        _logger.LogInformation("Booking {ReservationNumber} cancelled. Check-in: {CheckInDate}, Check-out: {CheckOutDate}, Final Amount: {FinalAmount}, Cancellation Fee ({FeePercent}%): {CancellationFee}, Refunded Amount: {RefundAmount}",
            booking.ReservationNumber, booking.CheckInDate, booking.CheckOutDate, booking.FinalAmount, 
            timeToCheckIn.TotalHours < 24 ? (int.TryParse(Environment.GetEnvironmentVariable("CANCELLATION_FEE_PERCENT"), out var f) ? f : 10) : 0, 
            cancellationFee, refundAmount);

        booking.Status = BookingStatus.Cancelled;

        if (booking.Payment != null)
        {
            if (booking.Payment.IsMock)
            {
                booking.PaymentStatus = PaymentStatus.Refunded;
            }
            else
            {
                // For real payments, would initiate Razorpay refund of the refundAmount
                booking.PaymentStatus = PaymentStatus.Refunded;
            }
        }

        await _bookingRepo.UpdateAsync(booking);

        // Revoke loyalty points
        try { await _loyaltyService.RevokePointsAsync(booking.UserId, booking.Id); }
        catch { /* Non-critical */ }

        // Send cancellation email
        try { await _emailService.SendCancellationAsync(booking); }
        catch { /* Non-critical */ }

        return MapToResponse(booking);
    }

    public async Task<BookingResponseDto> RebookAsync(int id, RebookDto dto, string userId)
    {
        var booking = await _bookingRepo.GetWithDetailsAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        if (booking.UserId != userId)
            throw new ForbiddenException("You can only rebook your own bookings.");

        if (booking.Status != BookingStatus.Cancelled)
            throw new Core.Exceptions.ValidationException("Only cancelled bookings can be rebooked.");

        // Create a new booking with the new dates
        var newDto = new CreateBookingDto
        {
            RoomId = dto.NewRoomId ?? booking.RoomId,
            CheckInDate = dto.NewCheckInDate,
            CheckOutDate = dto.NewCheckOutDate
        };

        return await CreateBookingAsync(newDto, userId);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync(
        BookingStatus? status, DateTime? from, DateTime? to, int? hotelId)
    {
        var bookings = await _bookingRepo.GetAllWithFiltersAsync(status, from, to, hotelId);
        return bookings.Select(MapToResponse);
    }

    public async Task<IEnumerable<BookingResponseDto>> GetHotelBookingsAsync(int hotelId, User currentUser)
    {
        if (currentUser.Role == UserRole.HotelAdmin && currentUser.HotelId != hotelId)
            throw new ForbiddenException("You can only view bookings for your assigned hotel.");

        var bookings = await _bookingRepo.GetAllWithFiltersAsync(null, null, null, hotelId);
        return bookings.Select(MapToResponse);
    }

    public async Task<BookingResponseDto> UpdateStatusAsync(int id, BookingStatus status)
    {
        var booking = await _bookingRepo.GetWithDetailsAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        booking.Status = status;
        await _bookingRepo.UpdateAsync(booking);

        return MapToResponse(booking);
    }

    private async Task<string> GenerateUniqueReservationNumber()
    {
        for (int i = 0; i < 5; i++)
        {
            var number = ReservationNumberGenerator.Generate();
            var exists = await _bookingRepo.ExistsAsync(b => b.ReservationNumber == number);
            if (!exists) return number;
        }
        throw new InvalidOperationException("Failed to generate unique reservation number after 5 attempts.");
    }

    private static BookingResponseDto MapToResponse(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            ReservationNumber = booking.ReservationNumber,
            HotelName = booking.Room?.RoomCategory?.Hotel?.Name ?? string.Empty,
            RoomNumber = booking.Room?.RoomNumber ?? string.Empty,
            RoomCategory = booking.Room?.RoomCategory?.Name.ToString() ?? string.Empty,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalNights = booking.TotalNights,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount,
            FinalAmount = booking.FinalAmount,
            Status = booking.Status.ToString(),
            PaymentStatus = booking.PaymentStatus.ToString(),
            CreatedAt = booking.CreatedAt,
            Payment = booking.Payment != null ? new PaymentResponseDto
            {
                Id = booking.Payment.Id,
                BookingId = booking.Payment.BookingId,
                OrderId = booking.Payment.RazorpayOrderId,
                Amount = booking.Payment.Amount,
                Currency = booking.Payment.Currency,
                Status = booking.Payment.Status.ToString(),
                IsMock = booking.Payment.IsMock,
                CreatedAt = booking.Payment.CreatedAt
            } : null
        };
    }
}
