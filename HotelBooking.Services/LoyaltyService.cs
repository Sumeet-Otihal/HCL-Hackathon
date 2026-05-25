using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Services;

public class LoyaltyService : ILoyaltyService
{
    private readonly IGenericRepository<LoyaltyPoint> _loyaltyRepo;
    private readonly UserManager<User> _userManager;

    public LoyaltyService(IGenericRepository<LoyaltyPoint> loyaltyRepo, UserManager<User> userManager)
    {
        _loyaltyRepo = loyaltyRepo;
        _userManager = userManager;
    }

    public async Task AwardPointsAsync(string userId, int bookingId, decimal finalAmount)
    {
        // 1 point per ₹100 (integer division)
        var points = (int)(finalAmount / 100);
        if (points <= 0) return;

        var loyaltyPoint = new LoyaltyPoint
        {
            UserId = userId,
            BookingId = bookingId,
            Points = points,
            Type = LoyaltyPointType.Earned,
            CreatedAt = DateTime.UtcNow
        };

        await _loyaltyRepo.AddAsync(loyaltyPoint);

        // Update user's total points
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.LoyaltyPoints += points;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task RevokePointsAsync(string userId, int bookingId)
    {
        var allPoints = await _loyaltyRepo.GetAllAsync();
        var earnedPoints = allPoints
            .Where(lp => lp.UserId == userId && lp.BookingId == bookingId && lp.Type == LoyaltyPointType.Earned)
            .Sum(lp => lp.Points);

        if (earnedPoints <= 0) return;

        var revokeEntry = new LoyaltyPoint
        {
            UserId = userId,
            BookingId = bookingId,
            Points = -earnedPoints,
            Type = LoyaltyPointType.Redeemed,
            CreatedAt = DateTime.UtcNow
        };

        await _loyaltyRepo.AddAsync(revokeEntry);

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.LoyaltyPoints = Math.Max(0, user.LoyaltyPoints - earnedPoints);
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<IEnumerable<LoyaltyPoint>> GetUserPointsAsync(string userId)
    {
        var allPoints = await _loyaltyRepo.GetAllAsync();
        return allPoints.Where(lp => lp.UserId == userId).OrderByDescending(lp => lp.CreatedAt);
    }

    public async Task<int> GetTotalPointsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.LoyaltyPoints ?? 0;
    }
}
