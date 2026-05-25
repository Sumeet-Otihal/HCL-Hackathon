using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
{
    public HotelRepository(AppDbContext context) : base(context) { }

    public async Task<Hotel?> GetWithRoomCategoriesAsync(int hotelId)
    {
        return await _dbSet
            .Include(h => h.RoomCategories)
                .ThenInclude(rc => rc.Rooms)
            .FirstOrDefaultAsync(h => h.Id == hotelId);
    }

    public async Task<PaginatedResult<Hotel>> SearchAsync(HotelSearchQueryDto query)
    {
        var q = _dbSet.AsQueryable();

        // City filter (case-insensitive partial match)
        if (!string.IsNullOrWhiteSpace(query.City))
            q = q.Where(h => h.City.ToLower().Contains(query.City.ToLower()));

        // Country filter
        if (!string.IsNullOrWhiteSpace(query.Country))
            q = q.Where(h => h.Country.ToLower().Contains(query.Country.ToLower()));

        // Star rating filter
        if (query.StarRating.HasValue)
            q = q.Where(h => h.StarRating == query.StarRating.Value);

        // Price range filter (via room categories)
        if (query.MinPrice.HasValue)
            q = q.Where(h => h.RoomCategories.Any(rc => rc.PricePerNight >= query.MinPrice.Value));

        if (query.MaxPrice.HasValue)
            q = q.Where(h => h.RoomCategories.Any(rc => rc.PricePerNight <= query.MaxPrice.Value));

        // Min occupancy filter
        if (query.MinOccupancy.HasValue)
            q = q.Where(h => h.RoomCategories.Any(rc => rc.MaxOccupancy >= query.MinOccupancy.Value));

        // Hotel amenities filter (JSON LIKE search)
        if (query.Amenities != null && query.Amenities.Count > 0)
        {
            foreach (var amenity in query.Amenities)
            {
                var term = amenity;
                q = q.Where(h => EF.Functions.Like(h.Amenities, $"%{term}%"));
            }
        }

        // Room amenities filter
        if (query.RoomAmenities != null && query.RoomAmenities.Count > 0)
        {
            foreach (var amenity in query.RoomAmenities)
            {
                var term = amenity;
                q = q.Where(h => h.RoomCategories.Any(rc => EF.Functions.Like(rc.Amenities, $"%{term}%")));
            }
        }

        // Availability filter (check-in/check-out dates)
        if (query.CheckIn.HasValue && query.CheckOut.HasValue)
        {
            q = q.Where(h => h.RoomCategories.Any(rc =>
                rc.Rooms.Any(r =>
                    !r.Bookings.Any(b =>
                        b.Status != BookingStatus.Cancelled &&
                        b.CheckInDate < query.CheckOut.Value &&
                        b.CheckOutDate > query.CheckIn.Value
                    )
                )
            ));
        }

        // Get total count before pagination
        var totalCount = await q.CountAsync();

        // Sorting
        q = query.SortBy?.ToLower() switch
        {
            "price_asc" => q.OrderBy(h => h.RoomCategories.Min(rc => rc.PricePerNight)),
            "price_desc" => q.OrderByDescending(h => h.RoomCategories.Min(rc => rc.PricePerNight)),
            "star_asc" => q.OrderBy(h => h.StarRating),
            "star_desc" => q.OrderByDescending(h => h.StarRating),
            "name_asc" => q.OrderBy(h => h.Name),
            _ => q.OrderBy(h => h.Name)
        };

        // Clamp pageSize
        var pageSize = Math.Min(Math.Max(query.PageSize, 1), 50);
        var page = Math.Max(query.Page, 1);

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(h => h.RoomCategories)
            .ToListAsync();

        return new PaginatedResult<Hotel>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
