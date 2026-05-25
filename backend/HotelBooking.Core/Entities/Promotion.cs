using HotelBooking.Core.Enums;

namespace HotelBooking.Core.Entities;

public class Promotion
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinBookingAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageLimit { get; set; }
    public int UsageCount { get; set; } = 0;
}
