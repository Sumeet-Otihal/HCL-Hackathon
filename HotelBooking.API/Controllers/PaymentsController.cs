using System.Security.Claims;
using HotelBooking.Core.DTOs.Payment;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-order")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var result = await _paymentService.CreateOrderAsync(dto.BookingId, dto.Amount);
        return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment order created."));
    }

    [HttpPost("verify")]
    [Authorize(Roles = Roles.User)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> Verify([FromBody] VerifyPaymentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _paymentService.VerifyPaymentAsync(dto, userId);
        return Ok(ApiResponse<PaymentResponseDto>.Ok(result, "Payment verified."));
    }

    [HttpGet("{bookingId:int}")]
    [Authorize(Roles = Roles.AnyAuthenticated)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetByBooking(int bookingId)
    {
        var result = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
        return Ok(ApiResponse<PaymentResponseDto>.Ok(result));
    }
}
