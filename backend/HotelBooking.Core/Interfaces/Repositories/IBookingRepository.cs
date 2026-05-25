using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Interfaces.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Booking>> GetAllWithFiltersAsync(BookingStatus? status, DateTime? from, DateTime? to, int? hotelId);
    Task<Booking?> GetWithDetailsAsync(int bookingId);
}
