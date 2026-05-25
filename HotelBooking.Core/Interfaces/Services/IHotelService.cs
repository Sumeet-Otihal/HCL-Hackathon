using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Helpers;

namespace HotelBooking.Core.Interfaces.Services;

public interface IHotelService
{
    Task<PaginatedResult<HotelResponseDto>> GetAllAsync(int page, int pageSize);
    Task<HotelResponseDto> GetByIdAsync(int id);
    Task<PaginatedResult<HotelResponseDto>> SearchAsync(HotelSearchQueryDto query);
    Task<HotelResponseDto> CreateAsync(CreateHotelDto dto);
    Task<HotelResponseDto> UpdateAsync(int id, UpdateHotelDto dto);
    Task DeleteAsync(int id);
}
