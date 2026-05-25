using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Entities;

public class LoyaltyPoint
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public int Points { get; set; }
    public LoyaltyPointType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
}
