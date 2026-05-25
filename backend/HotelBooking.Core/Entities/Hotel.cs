namespace HotelBooking.Core.Entities;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string Amenities { get; set; } = "[]";
    public string ImageUrls { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<RoomCategory> RoomCategories { get; set; } = new List<RoomCategory>();
}
