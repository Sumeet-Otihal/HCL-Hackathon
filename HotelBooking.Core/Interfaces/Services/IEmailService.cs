using HotelBooking.Core.Entities;

namespace HotelBooking.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(Booking booking);
    Task SendCancellationAsync(Booking booking);
}
