namespace HotelBooking.Core.DTOs.Hotel;

public class HotelSearchQueryDto
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? StarRating { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public int? MinOccupancy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
}
