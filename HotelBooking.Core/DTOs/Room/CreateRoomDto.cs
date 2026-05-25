namespace HotelBooking.Core.DTOs.Room;

public class CreateRoomDto
{
    public int RoomCategoryId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
}
