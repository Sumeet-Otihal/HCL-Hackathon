using System.Text.Json;
using AutoMapper;
using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.DTOs.Room;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;

namespace HotelBooking.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IGenericRepository<RoomCategory> _categoryRepo;
    private readonly IHotelRepository _hotelRepo;
    private readonly IMapper _mapper;

    public RoomService(
        IRoomRepository roomRepo,
        IGenericRepository<RoomCategory> categoryRepo,
        IHotelRepository hotelRepo,
        IMapper mapper)
    {
        _roomRepo = roomRepo;
        _categoryRepo = categoryRepo;
        _hotelRepo = hotelRepo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomCategoryResponseDto>> GetCategoriesByHotelAsync(int hotelId)
    {
        var hotel = await _hotelRepo.GetWithRoomCategoriesAsync(hotelId);
        if (hotel == null)
            throw new NotFoundException("Hotel", hotelId);

        return _mapper.Map<IEnumerable<RoomCategoryResponseDto>>(hotel.RoomCategories);
    }

    public async Task<IEnumerable<RoomResponseDto>> GetRoomsByCategoryAsync(int categoryId)
    {
        var rooms = await _roomRepo.GetByCategoryAsync(categoryId);
        return rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            RoomCategoryId = r.RoomCategoryId,
            CategoryName = r.RoomCategory.Name.ToString(),
            RoomNumber = r.RoomNumber,
            FloorNumber = r.FloorNumber,
            PricePerNight = r.RoomCategory.PricePerNight,
            IsAvailable = true // Would need date range for accurate check
        });
    }

    public async Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return await _roomRepo.IsAvailableAsync(roomId, checkIn, checkOut);
    }

    public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, User currentUser)
    {
        var category = await _categoryRepo.GetByIdAsync(dto.RoomCategoryId);
        if (category == null)
            throw new NotFoundException("RoomCategory", dto.RoomCategoryId);

        EnforceHotelScope(currentUser, category.HotelId);

        var room = new Room
        {
            RoomCategoryId = dto.RoomCategoryId,
            RoomNumber = dto.RoomNumber,
            FloorNumber = dto.FloorNumber,
            IsActive = true
        };

        var created = await _roomRepo.AddAsync(room);
        return new RoomResponseDto
        {
            Id = created.Id,
            RoomCategoryId = created.RoomCategoryId,
            CategoryName = category.Name.ToString(),
            RoomNumber = created.RoomNumber,
            FloorNumber = created.FloorNumber,
            PricePerNight = category.PricePerNight,
            IsAvailable = true
        };
    }

    public async Task<RoomResponseDto> UpdateRoomAsync(int id, UpdateRoomDto dto, User currentUser)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room == null)
            throw new NotFoundException("Room", id);

        var category = await _categoryRepo.GetByIdAsync(room.RoomCategoryId);
        if (category == null)
            throw new NotFoundException("RoomCategory", room.RoomCategoryId);

        EnforceHotelScope(currentUser, category.HotelId);

        room.RoomNumber = dto.RoomNumber;
        room.FloorNumber = dto.FloorNumber;

        await _roomRepo.UpdateAsync(room);
        return new RoomResponseDto
        {
            Id = room.Id,
            RoomCategoryId = room.RoomCategoryId,
            CategoryName = category.Name.ToString(),
            RoomNumber = room.RoomNumber,
            FloorNumber = room.FloorNumber,
            PricePerNight = category.PricePerNight,
            IsAvailable = true
        };
    }

    public async Task DeleteRoomAsync(int id, User currentUser)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room == null)
            throw new NotFoundException("Room", id);

        var category = await _categoryRepo.GetByIdAsync(room.RoomCategoryId);
        if (category != null) EnforceHotelScope(currentUser, category.HotelId);

        room.IsActive = false;
        room.DeletedAt = DateTime.UtcNow;
        await _roomRepo.UpdateAsync(room);
    }

    public async Task<RoomCategoryResponseDto> CreateCategoryAsync(CreateRoomCategoryDto dto, User currentUser)
    {
        EnforceHotelScope(currentUser, dto.HotelId);

        var hotel = await _hotelRepo.GetByIdAsync(dto.HotelId);
        if (hotel == null)
            throw new NotFoundException("Hotel", dto.HotelId);

        var category = new RoomCategory
        {
            HotelId = dto.HotelId,
            Name = dto.Name,
            Description = dto.Description,
            PricePerNight = dto.PricePerNight,
            MaxOccupancy = dto.MaxOccupancy,
            Amenities = JsonSerializer.Serialize(dto.Amenities),
            ImageUrls = JsonSerializer.Serialize(dto.ImageUrls),
            IsActive = true
        };

        var created = await _categoryRepo.AddAsync(category);
        return _mapper.Map<RoomCategoryResponseDto>(created);
    }

    public async Task<RoomCategoryResponseDto> UpdateCategoryAsync(int id, UpdateRoomCategoryDto dto, User currentUser)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
            throw new NotFoundException("RoomCategory", id);

        EnforceHotelScope(currentUser, category.HotelId);

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.PricePerNight = dto.PricePerNight;
        category.MaxOccupancy = dto.MaxOccupancy;
        category.Amenities = JsonSerializer.Serialize(dto.Amenities);
        category.ImageUrls = JsonSerializer.Serialize(dto.ImageUrls);

        await _categoryRepo.UpdateAsync(category);
        return _mapper.Map<RoomCategoryResponseDto>(category);
    }

    public async Task DeleteCategoryAsync(int id, User currentUser)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
            throw new NotFoundException("RoomCategory", id);

        EnforceHotelScope(currentUser, category.HotelId);

        category.IsActive = false;
        category.DeletedAt = DateTime.UtcNow;
        await _categoryRepo.UpdateAsync(category);
    }

    private void EnforceHotelScope(User currentUser, int targetHotelId)
    {
        if (currentUser.Role == UserRole.HotelAdmin && currentUser.HotelId != targetHotelId)
            throw new ForbiddenException("You can only manage resources for your assigned hotel.");
    }
}
