using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Helpers;

namespace HotelBooking.Core.Interfaces.Repositories;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<PaginatedResult<Hotel>> SearchAsync(HotelSearchQueryDto query);
    Task<Hotel?> GetWithRoomCategoriesAsync(int hotelId);
}
