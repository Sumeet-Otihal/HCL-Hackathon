using HotelBooking.Core.Enums;

namespace HotelBooking.Core.DTOs.Room;

public class UpdateRoomCategoryDto
{
    public RoomCategoryName Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}
