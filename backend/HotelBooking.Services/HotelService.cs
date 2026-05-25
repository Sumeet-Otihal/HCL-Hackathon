using System.Text.Json;
using AutoMapper;
using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;

namespace HotelBooking.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepo;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepo, IMapper mapper)
    {
        _hotelRepo = hotelRepo;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<HotelResponseDto>> GetAllAsync(int page, int pageSize)
    {
        var query = new HotelSearchQueryDto { Page = page, PageSize = pageSize };
        var result = await _hotelRepo.SearchAsync(query);

        return new PaginatedResult<HotelResponseDto>
        {
            Items = _mapper.Map<IEnumerable<HotelResponseDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<HotelResponseDto> GetByIdAsync(int id)
    {
        var hotel = await _hotelRepo.GetWithRoomCategoriesAsync(id);
        if (hotel == null)
            throw new NotFoundException("Hotel", id);

        return _mapper.Map<HotelResponseDto>(hotel);
    }

    public async Task<PaginatedResult<HotelResponseDto>> SearchAsync(HotelSearchQueryDto query)
    {
        var result = await _hotelRepo.SearchAsync(query);

        return new PaginatedResult<HotelResponseDto>
        {
            Items = _mapper.Map<IEnumerable<HotelResponseDto>>(result.Items),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<HotelResponseDto> CreateAsync(CreateHotelDto dto)
    {
        var hotel = _mapper.Map<Hotel>(dto);
        hotel.CreatedAt = DateTime.UtcNow;
        hotel.IsActive = true;

        var created = await _hotelRepo.AddAsync(hotel);
        return _mapper.Map<HotelResponseDto>(created);
    }

    public async Task<HotelResponseDto> UpdateAsync(int id, UpdateHotelDto dto)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id);
        if (hotel == null)
            throw new NotFoundException("Hotel", id);

        hotel.Name = dto.Name;
        hotel.Description = dto.Description;
        hotel.Address = dto.Address;
        hotel.City = dto.City;
        hotel.Country = dto.Country;
        hotel.StarRating = dto.StarRating;
        hotel.Amenities = JsonSerializer.Serialize(dto.Amenities);
        hotel.ImageUrls = JsonSerializer.Serialize(dto.ImageUrls);
        hotel.UpdatedAt = DateTime.UtcNow;

        await _hotelRepo.UpdateAsync(hotel);
        return _mapper.Map<HotelResponseDto>(hotel);
    }

    public async Task DeleteAsync(int id)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id);
        if (hotel == null)
            throw new NotFoundException("Hotel", id);

        // Soft delete — never hard delete
        hotel.IsActive = false;
        hotel.DeletedAt = DateTime.UtcNow;
        await _hotelRepo.UpdateAsync(hotel);
    }
}
