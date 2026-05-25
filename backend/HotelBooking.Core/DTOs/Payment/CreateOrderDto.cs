namespace HotelBooking.Core.DTOs.Payment;

public class CreateOrderDto
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
}
