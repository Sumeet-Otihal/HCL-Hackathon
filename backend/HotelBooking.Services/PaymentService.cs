using System.Security.Cryptography;
using System.Text;
using HotelBooking.Core.DTOs.Payment;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Helpers.Settings;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Services;

public class PaymentService : IPaymentService
{
    private readonly IGenericRepository<Payment> _paymentRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly ILoyaltyService _loyaltyService;
    private readonly IEmailService _emailService;
    private readonly RazorpaySettings _razorpaySettings;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IGenericRepository<Payment> paymentRepo,
        IBookingRepository bookingRepo,
        ILoyaltyService loyaltyService,
        IEmailService emailService,
        RazorpaySettings razorpaySettings,
        ILogger<PaymentService> logger)
    {
        _paymentRepo = paymentRepo;
        _bookingRepo = bookingRepo;
        _loyaltyService = loyaltyService;
        _emailService = emailService;
        _razorpaySettings = razorpaySettings;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> CreateOrderAsync(int bookingId, decimal amount)
    {
        string? orderId = null;
        bool isMock = true;

        if (_razorpaySettings.Enabled)
        {
            try
            {
                var client = new Razorpay.Api.RazorpayClient(_razorpaySettings.KeyId, _razorpaySettings.KeySecret);
                var options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) }, // amount in paise
                    { "currency", _razorpaySettings.Currency },
                    { "receipt", bookingId.ToString() }
                };

                var order = client.Order.Create(options);
                orderId = order["id"].ToString();
                isMock = false;
                _logger.LogInformation("Successfully created Razorpay order {OrderId} for booking {BookingId}", orderId, bookingId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Razorpay order creation failed, falling through to mock.");
            }
        }

        // Mock mode (default or fallback)
        if (orderId == null)
        {
            orderId = $"MOCK_ORDER_{Guid.NewGuid():N}";
            isMock = true;
        }

        var payment = new Payment
        {
            BookingId = bookingId,
            RazorpayOrderId = orderId,
            Amount = amount,
            Currency = _razorpaySettings.Currency,
            Status = PaymentResult.Mock,
            IsMock = isMock,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepo.AddAsync(payment);

        return new PaymentResponseDto
        {
            Id = payment.Id,
            BookingId = bookingId,
            OrderId = orderId,
            Amount = amount,
            Currency = _razorpaySettings.Currency,
            Status = payment.Status.ToString(),
            IsMock = isMock,
            Key = isMock ? null : _razorpaySettings.KeyId,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<PaymentResponseDto> VerifyPaymentAsync(VerifyPaymentDto dto, string userId)
    {
        var payment = (await _paymentRepo.GetAllAsync())
            .FirstOrDefault(p => p.BookingId == dto.BookingId);

        if (payment == null)
            throw new NotFoundException("Payment", dto.BookingId);

        if (payment.IsMock)
        {
            // Mock mode — auto approve
            payment.Status = PaymentResult.Mock;
            payment.RazorpayPaymentId = $"MOCK_PAY_{Guid.NewGuid():N}";
        }
        else
        {
            // Real Razorpay — verify HMAC-SHA256 signature
            var expectedSignature = ComputeHmacSha256(
                $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}",
                _razorpaySettings.KeySecret);

            if (expectedSignature != dto.RazorpaySignature)
            {
                payment.Status = PaymentResult.Failed;
                await _paymentRepo.UpdateAsync(payment);
                throw new Core.Exceptions.ValidationException("Payment verification failed — invalid signature.");
            }

            payment.RazorpayPaymentId = dto.RazorpayPaymentId;
            payment.RazorpaySignature = dto.RazorpaySignature;
            payment.Status = PaymentResult.Success;
        }

        await _paymentRepo.UpdateAsync(payment);

        // Update booking status
        var booking = await _bookingRepo.GetWithDetailsAsync(dto.BookingId);
        if (booking != null)
        {
            booking.Status = BookingStatus.Confirmed;
            booking.PaymentStatus = payment.IsMock ? PaymentStatus.MockPaid : PaymentStatus.Paid;
            await _bookingRepo.UpdateAsync(booking);

            // Award loyalty points: 1 point per ₹100
            try { await _loyaltyService.AwardPointsAsync(userId, booking.Id, booking.FinalAmount); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to award loyalty points for booking {BookingId}", booking.Id); }

            // Send confirmation email
            try { await _emailService.SendBookingConfirmationAsync(booking); }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to send confirmation email for booking {BookingId}", booking.Id); }
        }

        return new PaymentResponseDto
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            OrderId = payment.RazorpayOrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            IsMock = payment.IsMock,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<PaymentResponseDto> GetPaymentByBookingIdAsync(int bookingId)
    {
        var payment = (await _paymentRepo.GetAllAsync())
            .FirstOrDefault(p => p.BookingId == bookingId);

        if (payment == null)
            throw new NotFoundException("Payment for booking", bookingId);

        return new PaymentResponseDto
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            OrderId = payment.RazorpayOrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            IsMock = payment.IsMock,
            CreatedAt = payment.CreatedAt
        };
    }

    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
