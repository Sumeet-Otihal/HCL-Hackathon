using HotelBooking.Core.DTOs.Payment;

namespace HotelBooking.Core.Interfaces.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreateOrderAsync(int bookingId, decimal amount);
    Task<PaymentResponseDto> VerifyPaymentAsync(VerifyPaymentDto dto, string userId);
    Task<PaymentResponseDto> GetPaymentByBookingIdAsync(int bookingId);
}
