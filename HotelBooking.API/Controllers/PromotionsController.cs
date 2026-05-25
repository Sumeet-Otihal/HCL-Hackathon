using HotelBooking.Core.Entities;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpPost("validate")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<decimal>>> Validate(
        [FromQuery] string code, [FromQuery] decimal totalAmount)
    {
        var discount = await _promotionService.ValidateAndApplyAsync(code, totalAmount);
        return Ok(ApiResponse<decimal>.Ok(discount, "Promotion code is valid."));
    }

    [HttpGet]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Promotion>>>> GetAll()
    {
        var result = await _promotionService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<Promotion>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<Promotion>>> Create([FromBody] Promotion promotion)
    {
        var result = await _promotionService.CreateAsync(promotion);
        return Ok(ApiResponse<Promotion>.Ok(result, "Promotion created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<Promotion>>> Update(int id, [FromBody] Promotion promotion)
    {
        var result = await _promotionService.UpdateAsync(id, promotion);
        return Ok(ApiResponse<Promotion>.Ok(result, "Promotion updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        await _promotionService.DeleteAsync(id);
        return Ok(ApiResponse<string>.Ok("Promotion deactivated."));
    }
}
