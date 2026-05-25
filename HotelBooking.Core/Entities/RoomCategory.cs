using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Entities;

public class RoomCategory
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public RoomCategoryName Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public string Amenities { get; set; } = "[]";
    public string ImageUrls { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
