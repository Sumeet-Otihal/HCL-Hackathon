using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Room>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(r => r.RoomCategory)
            .Where(r => r.RoomCategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<bool> IsAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var hasOverlap = await _context.Bookings.AnyAsync(b =>
            (b.RoomId == roomId || b.BookingRooms.Any(br => br.RoomId == roomId)) &&
            b.Status != BookingStatus.Cancelled &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn
        );

        return !hasOverlap;
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int minOccupancy)
    {
        return await _dbSet
            .Include(r => r.RoomCategory)
            .Where(r => r.RoomCategory.HotelId == hotelId)
            .Where(r => r.RoomCategory.MaxOccupancy >= minOccupancy)
            .Where(r => !r.Bookings.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.CheckInDate < checkOut &&
                b.CheckOutDate > checkIn
            ) && !_context.BookingRooms.Any(br =>
                br.RoomId == r.Id &&
                br.Booking.Status != BookingStatus.Cancelled &&
                br.Booking.CheckInDate < checkOut &&
                br.Booking.CheckOutDate > checkIn
            ))
            .ToListAsync();
    }
}
