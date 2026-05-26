using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Entities;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public int RoomCount { get; set; } = 1;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int TotalNights { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public decimal FinalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string ReservationNumber { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
    public Payment? Payment { get; set; }
    public ICollection<LoyaltyPoint> LoyaltyPoints { get; set; } = new List<LoyaltyPoint>();
}
