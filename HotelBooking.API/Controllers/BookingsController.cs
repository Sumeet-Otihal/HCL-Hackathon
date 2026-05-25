using System.Security.Claims;
using HotelBooking.Core.DTOs.Booking;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly UserManager<User> _userManager;

    public BookingsController(IBookingService bookingService, UserManager<User> userManager)
    {
        _bookingService = bookingService;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> Create([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookingService.CreateBookingAsync(dto, userId);
        return Ok(ApiResponse<BookingResponseDto>.Ok(result, "Booking created successfully."));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.AnyAuthenticated)]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> GetById(int id)
    {
        var currentUser = await GetCurrentUser();
        var result = await _bookingService.GetBookingAsync(id, currentUser);
        return Ok(ApiResponse<BookingResponseDto>.Ok(result));
    }

    [HttpGet("my-bookings")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingResponseDto>>>> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookingService.GetMyBookingsAsync(userId);
        return Ok(ApiResponse<IEnumerable<BookingResponseDto>>.Ok(result));
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = Roles.AnyAuthenticated)]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> Cancel(int id)
    {
        var currentUser = await GetCurrentUser();
        var result = await _bookingService.CancelBookingAsync(id, currentUser);
        return Ok(ApiResponse<BookingResponseDto>.Ok(result, "Booking cancelled successfully."));
    }

    [HttpPost("{id:int}/rebook")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> Rebook(int id, [FromBody] RebookDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookingService.RebookAsync(id, dto, userId);
        return Ok(ApiResponse<BookingResponseDto>.Ok(result, "Rebooking successful."));
    }

    [HttpGet]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingResponseDto>>>> GetAll(
        [FromQuery] BookingStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? hotelId)
    {
        var result = await _bookingService.GetAllBookingsAsync(status, from, to, hotelId);
        return Ok(ApiResponse<IEnumerable<BookingResponseDto>>.Ok(result));
    }

    [HttpGet("hotel/{hotelId:int}")]
    [Authorize(Roles = Roles.SuperAdminOrHotelAdmin)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingResponseDto>>>> GetByHotel(int hotelId)
    {
        var currentUser = await GetCurrentUser();
        var result = await _bookingService.GetHotelBookingsAsync(hotelId, currentUser);
        return Ok(ApiResponse<IEnumerable<BookingResponseDto>>.Ok(result));
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<BookingResponseDto>>> UpdateStatus(
        int id, [FromQuery] BookingStatus status)
    {
        var result = await _bookingService.UpdateStatusAsync(id, status);
        return Ok(ApiResponse<BookingResponseDto>.Ok(result, "Booking status updated."));
    }

    private async Task<User> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return (await _userManager.FindByIdAsync(userId))!;
    }
}
