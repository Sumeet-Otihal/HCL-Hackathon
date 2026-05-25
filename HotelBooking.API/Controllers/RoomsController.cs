using System.Security.Claims;
using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.DTOs.Room;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly UserManager<User> _userManager;

    public RoomsController(IRoomService roomService, UserManager<User> userManager)
    {
        _roomService = roomService;
        _userManager = userManager;
    }

    [HttpGet("hotel/{hotelId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoomCategoryResponseDto>>>> GetCategoriesByHotel(int hotelId)
    {
        var result = await _roomService.GetCategoriesByHotelAsync(hotelId);
        return Ok(ApiResponse<IEnumerable<RoomCategoryResponseDto>>.Ok(result));
    }

    [HttpGet("{id:int}/availability")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckAvailability(
        int id, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
    {
        var result = await _roomService.CheckAvailabilityAsync(id, checkIn, checkOut);
        return Ok(ApiResponse<bool>.Ok(result));
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoomResponseDto>>>> GetByCategory(int categoryId)
    {
        var result = await _roomService.GetRoomsByCategoryAsync(categoryId);
        return Ok(ApiResponse<IEnumerable<RoomResponseDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Create([FromBody] CreateRoomDto dto)
    {
        var currentUser = await GetCurrentUser();
        var result = await _roomService.CreateRoomAsync(dto, currentUser);
        return Ok(ApiResponse<RoomResponseDto>.Ok(result, "Room created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<RoomResponseDto>>> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        var currentUser = await GetCurrentUser();
        var result = await _roomService.UpdateRoomAsync(id, dto, currentUser);
        return Ok(ApiResponse<RoomResponseDto>.Ok(result, "Room updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var currentUser = await GetCurrentUser();
        await _roomService.DeleteRoomAsync(id, currentUser);
        return Ok(ApiResponse<string>.Ok("Room deleted successfully."));
    }

    [HttpPost("categories")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<RoomCategoryResponseDto>>> CreateCategory([FromBody] CreateRoomCategoryDto dto)
    {
        var currentUser = await GetCurrentUser();
        var result = await _roomService.CreateCategoryAsync(dto, currentUser);
        return Ok(ApiResponse<RoomCategoryResponseDto>.Ok(result, "Room category created successfully."));
    }

    [HttpPut("categories/{id:int}")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<RoomCategoryResponseDto>>> UpdateCategory(int id, [FromBody] UpdateRoomCategoryDto dto)
    {
        var currentUser = await GetCurrentUser();
        var result = await _roomService.UpdateCategoryAsync(id, dto, currentUser);
        return Ok(ApiResponse<RoomCategoryResponseDto>.Ok(result, "Room category updated successfully."));
    }

    [HttpDelete("categories/{id:int}")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
    {
        var currentUser = await GetCurrentUser();
        await _roomService.DeleteCategoryAsync(id, currentUser);
        return Ok(ApiResponse<string>.Ok("Room category deleted successfully."));
    }

    private async Task<User> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return (await _userManager.FindByIdAsync(userId))!;
    }
}
