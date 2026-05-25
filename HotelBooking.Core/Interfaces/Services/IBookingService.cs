using HotelBooking.Core.DTOs.Booking;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Interfaces.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, string userId);
    Task<BookingResponseDto> GetBookingAsync(int id, User currentUser);
    Task<IEnumerable<BookingResponseDto>> GetMyBookingsAsync(string userId);
    Task<BookingResponseDto> CancelBookingAsync(int id, User currentUser);
    Task<BookingResponseDto> RebookAsync(int id, RebookDto dto, string userId);
    Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync(BookingStatus? status, DateTime? from, DateTime? to, int? hotelId);
    Task<IEnumerable<BookingResponseDto>> GetHotelBookingsAsync(int hotelId, User currentUser);
    Task<BookingResponseDto> UpdateStatusAsync(int id, BookingStatus status);
}
