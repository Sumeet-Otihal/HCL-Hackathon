using HotelBooking.Core.DTOs.Auth;
using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IBookingRepository _bookingRepo;
    private readonly IHotelRepository _hotelRepo;

    public AdminController(
        UserManager<User> userManager,
        IBookingRepository bookingRepo,
        IHotelRepository hotelRepo)
    {
        _userManager = userManager;
        _bookingRepo = bookingRepo;
        _hotelRepo = hotelRepo;
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserInfoDto>>>> GetAllUsers(
        [FromQuery] string? role)
    {
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
        {
            if (Enum.TryParse<UserRole>(role, true, out var userRole))
                users = users.Where(u => u.Role == userRole);
        }

        var userList = await users.ToListAsync();
        var result = new List<UserInfoDto>();

        foreach (var user in userList)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? Roles.User,
                LoyaltyPoints = user.LoyaltyPoints
            });
        }

        return Ok(ApiResponse<IEnumerable<UserInfoDto>>.Ok(result));
    }

    [HttpPut("users/{id}/role")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateRole(string id, [FromQuery] string newRole)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new NotFoundException("User", id);

        // Remove current roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Add new role
        await _userManager.AddToRoleAsync(user, newRole);

        if (Enum.TryParse<UserRole>(newRole, true, out var userRole))
            user.Role = userRole;

        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<string>.Ok($"User role updated to {newRole}."));
    }

    [HttpPut("users/{id}/hotel")]
    public async Task<ActionResult<ApiResponse<string>>> AssignHotel(string id, [FromQuery] int hotelId)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new NotFoundException("User", id);

        var hotel = await _hotelRepo.GetByIdAsync(hotelId);
        if (hotel == null) throw new NotFoundException("Hotel", hotelId);

        user.HotelId = hotelId;
        user.Role = UserRole.HotelAdmin;

        // Ensure HotelAdmin role
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(Roles.HotelAdmin))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Roles.HotelAdmin);
        }

        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<string>.Ok($"User assigned to hotel {hotel.Name} as HotelAdmin."));
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeactivateUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) throw new NotFoundException("User", id);

        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<string>.Ok("User deactivated successfully."));
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var totalHotels = (await _hotelRepo.GetAllAsync()).Count();
        var allBookings = await _bookingRepo.GetAllWithFiltersAsync(null, null, null, null);
        var bookingList = allBookings.ToList();

        var stats = new
        {
            TotalUsers = totalUsers,
            TotalHotels = totalHotels,
            TotalBookings = bookingList.Count,
            ConfirmedBookings = bookingList.Count(b => b.Status == BookingStatus.Confirmed),
            CancelledBookings = bookingList.Count(b => b.Status == BookingStatus.Cancelled),
            TotalRevenue = bookingList
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .Sum(b => b.FinalAmount)
        };

        return Ok(ApiResponse<object>.Ok(stats));
    }
}
