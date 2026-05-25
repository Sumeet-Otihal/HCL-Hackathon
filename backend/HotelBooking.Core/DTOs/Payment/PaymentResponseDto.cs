namespace HotelBooking.Core.DTOs.Payment;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string? OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsMock { get; set; }
    public string? Key { get; set; }
    public DateTime CreatedAt { get; set; }
}
