namespace HotelBooking.Core.DTOs.Room;

public class RoomResponseDto
{
    public int Id { get; set; }
    public int RoomCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
}
