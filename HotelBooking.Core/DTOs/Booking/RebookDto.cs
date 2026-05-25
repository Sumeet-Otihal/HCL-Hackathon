namespace HotelBooking.Core.DTOs.Booking;

public class RebookDto
{
    public DateTime NewCheckInDate { get; set; }
    public DateTime NewCheckOutDate { get; set; }
    public int? NewRoomId { get; set; }
}
