using System.Text.Json;
using AutoMapper;
using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;

namespace HotelBooking.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Hotel mappings
        CreateMap<Hotel, HotelResponseDto>()
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => DeserializeJson(src.Amenities)))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => DeserializeJson(src.ImageUrls)))
            .ForMember(dest => dest.RoomCategories, opt => opt.MapFrom(src => src.RoomCategories));

        CreateMap<CreateHotelDto, Hotel>()
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.Amenities, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.ImageUrls, (JsonSerializerOptions?)null)));

        CreateMap<UpdateHotelDto, Hotel>()
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.Amenities, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.ImageUrls, (JsonSerializerOptions?)null)));

        // RoomCategory mappings
        CreateMap<RoomCategory, RoomCategoryResponseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToString()))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => DeserializeJson(src.Amenities)))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => DeserializeJson(src.ImageUrls)))
            .ForMember(dest => dest.AvailableRooms, opt => opt.MapFrom(src => src.Rooms != null ? src.Rooms.Count(r => r.IsActive) : 0));
    }

    private static List<string> DeserializeJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
