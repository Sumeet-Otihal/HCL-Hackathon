namespace HotelBooking.Core.DTOs.Hotel;

public class HotelResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public List<RoomCategoryResponseDto>? RoomCategories { get; set; }
}

public class RoomCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public int AvailableRooms { get; set; }
}
