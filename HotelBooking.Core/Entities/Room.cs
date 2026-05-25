namespace HotelBooking.Core.Entities;

public class Room
{
    public int Id { get; set; }
    public int RoomCategoryId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public RoomCategory RoomCategory { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
