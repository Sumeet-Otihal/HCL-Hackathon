namespace HotelBooking.Core.DTOs.Payment;

public class VerifyPaymentDto
{
    public int BookingId { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
}
