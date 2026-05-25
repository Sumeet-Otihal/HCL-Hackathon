using HotelBooking.Core.Entities;

namespace HotelBooking.Core.Interfaces.Services;

public interface ILoyaltyService
{
    Task AwardPointsAsync(string userId, int bookingId, decimal finalAmount);
    Task RevokePointsAsync(string userId, int bookingId);
    Task<IEnumerable<LoyaltyPoint>> GetUserPointsAsync(string userId);
    Task<int> GetTotalPointsAsync(string userId);
}
