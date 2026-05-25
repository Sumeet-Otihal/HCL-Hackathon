namespace HotelBooking.Core.DTOs.Booking;

public class CreateBookingDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? PromoCode { get; set; }
}
