using HotelBooking.Core.Entities;

namespace HotelBooking.Core.Interfaces.Repositories;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<IEnumerable<Room>> GetByCategoryAsync(int categoryId);
    Task<bool> IsAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut, int minOccupancy);
}
