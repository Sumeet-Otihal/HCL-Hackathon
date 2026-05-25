namespace HotelBooking.Core.Helpers.Settings;

public class RazorpaySettings
{
    public bool Enabled { get; set; }
    public string KeyId { get; set; } = string.Empty;
    public string KeySecret { get; set; } = string.Empty;
    public string Currency { get; set; } = "INR";
}
