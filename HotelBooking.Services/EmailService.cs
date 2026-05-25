using HotelBooking.Core.Entities;
using HotelBooking.Core.Helpers.Settings;
using HotelBooking.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace HotelBooking.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(EmailSettings emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(Booking booking)
    {
        if (!_emailSettings.Enabled)
        {
            _logger.LogInformation("Email disabled — skipping booking confirmation for {ReservationNumber}", booking.ReservationNumber);
            return;
        }

        try
        {
            var subject = $"Booking Confirmed — {booking.ReservationNumber}";
            var body = $@"
                <h2>Booking Confirmation</h2>
                <p>Dear Customer,</p>
                <p>Your booking has been confirmed!</p>
                <ul>
                    <li><strong>Reservation Number:</strong> {booking.ReservationNumber}</li>
                    <li><strong>Check-in:</strong> {booking.CheckInDate:MMMM dd, yyyy}</li>
                    <li><strong>Check-out:</strong> {booking.CheckOutDate:MMMM dd, yyyy}</li>
                    <li><strong>Total Nights:</strong> {booking.TotalNights}</li>
                    <li><strong>Total Amount:</strong> ₹{booking.FinalAmount:N2}</li>
                </ul>
                <p>Thank you for choosing us!</p>";

            var userEmail = booking.User?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                await SendEmailAsync(userEmail, subject, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking confirmation email for {ReservationNumber}", booking.ReservationNumber);
            // Never throw — email is non-critical
        }
    }

    public async Task SendCancellationAsync(Booking booking)
    {
        if (!_emailSettings.Enabled)
        {
            _logger.LogInformation("Email disabled — skipping cancellation email for {ReservationNumber}", booking.ReservationNumber);
            return;
        }

        try
        {
            var refundAmount = booking.FinalAmount;
            var cancellationFee = 0m;
            var timeToCheckIn = booking.CheckInDate - DateTime.UtcNow;
            var feePercent = 0;
            if (timeToCheckIn.TotalHours < 24)
            {
                feePercent = 10;
                if (int.TryParse(Environment.GetEnvironmentVariable("CANCELLATION_FEE_PERCENT"), out var customFee))
                {
                    feePercent = customFee;
                }
                cancellationFee = booking.FinalAmount * (feePercent / 100m);
                refundAmount = booking.FinalAmount - cancellationFee;
            }

            var subject = $"Booking Cancelled — {booking.ReservationNumber}";
            var body = $@"
                <h2>Booking Cancellation</h2>
                <p>Dear Customer,</p>
                <p>Your booking {booking.ReservationNumber} has been cancelled.</p>
                <ul>
                    <li><strong>Original Paid Amount:</strong> ₹{booking.FinalAmount:N2}</li>
                    <li><strong>Cancellation Fee ({feePercent}%):</strong> ₹{cancellationFee:N2}</li>
                    <li><strong>Refund Amount:</strong> ₹{refundAmount:N2}</li>
                </ul>
                <p>If a refund is applicable, it will be processed within 5-7 business days.</p>
                <p>Thank you.</p>";

            var userEmail = booking.User?.Email;
            if (!string.IsNullOrEmpty(userEmail))
            {
                await SendEmailAsync(userEmail, subject, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancellation email for {ReservationNumber}", booking.ReservationNumber);
        }
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromAddress));
        message.To.Add(new MailboxAddress(string.Empty, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
    }
}
