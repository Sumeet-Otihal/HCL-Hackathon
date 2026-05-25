using HotelBooking.Core.DTOs.Hotel;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<HotelResponseDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _hotelService.GetAllAsync(page, pageSize);
        return Ok(ApiResponse<PaginatedResult<HotelResponseDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<HotelResponseDto>>> GetById(int id)
    {
        var result = await _hotelService.GetByIdAsync(id);
        return Ok(ApiResponse<HotelResponseDto>.Ok(result));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<HotelResponseDto>>>> Search(
        [FromQuery] HotelSearchQueryDto query)
    {
        var result = await _hotelService.SearchAsync(query);
        return Ok(ApiResponse<PaginatedResult<HotelResponseDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<HotelResponseDto>>> Create([FromBody] CreateHotelDto dto)
    {
        var result = await _hotelService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<HotelResponseDto>.Ok(result, "Hotel created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<HotelResponseDto>>> Update(int id, [FromBody] UpdateHotelDto dto)
    {
        var result = await _hotelService.UpdateAsync(id, dto);
        return Ok(ApiResponse<HotelResponseDto>.Ok(result, "Hotel updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await _hotelService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Hotel deleted successfully."));
    }
}
