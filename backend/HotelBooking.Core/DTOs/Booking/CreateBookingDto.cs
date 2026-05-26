namespace HotelBooking.Core.DTOs.Booking;

public class CreateBookingDto
{
    public int? RoomId { get; set; }
    public int CategoryId { get; set; }
    public int HotelId { get; set; }
    public int RoomCount { get; set; } = 1;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? PromoCode { get; set; }
}
