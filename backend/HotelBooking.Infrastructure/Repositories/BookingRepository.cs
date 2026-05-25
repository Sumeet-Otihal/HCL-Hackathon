using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomCategory)
                    .ThenInclude(rc => rc.Hotel)
            .Include(b => b.Payment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetAllWithFiltersAsync(BookingStatus? status, DateTime? from, DateTime? to, int? hotelId)
    {
        var query = _dbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomCategory)
                    .ThenInclude(rc => rc.Hotel)
            .Include(b => b.Payment)
            .Include(b => b.User)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        if (from.HasValue)
            query = query.Where(b => b.CheckInDate >= from.Value);

        if (to.HasValue)
            query = query.Where(b => b.CheckOutDate <= to.Value);

        if (hotelId.HasValue)
            query = query.Where(b => b.Room.RoomCategory.HotelId == hotelId.Value);

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<Booking?> GetWithDetailsAsync(int bookingId)
    {
        return await _dbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomCategory)
                    .ThenInclude(rc => rc.Hotel)
            .Include(b => b.Payment)
            .Include(b => b.User)
            .Include(b => b.LoyaltyPoints)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }
}
