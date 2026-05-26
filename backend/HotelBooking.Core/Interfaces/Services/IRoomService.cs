using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.DTOs.Room;
using HotelBooking.Core.Entities;

namespace HotelBooking.Core.Interfaces.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomCategoryResponseDto>> GetCategoriesByHotelAsync(int hotelId);
    Task<RoomCategoryResponseDto> GetCategoryByIdAsync(int id);
    Task<IEnumerable<RoomResponseDto>> GetRoomsByCategoryAsync(int categoryId);
    Task<IEnumerable<RoomResponseDto>> GetRoomsByHotelAsync(int hotelId, DateTime? checkIn = null, DateTime? checkOut = null);
    Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, User currentUser);
    Task<RoomResponseDto> UpdateRoomAsync(int id, UpdateRoomDto dto, User currentUser);
    Task DeleteRoomAsync(int id, User currentUser);
    Task<RoomCategoryResponseDto> CreateCategoryAsync(CreateRoomCategoryDto dto, User currentUser);
    Task<RoomCategoryResponseDto> UpdateCategoryAsync(int id, UpdateRoomCategoryDto dto, User currentUser);
    Task DeleteCategoryAsync(int id, User currentUser);
}
