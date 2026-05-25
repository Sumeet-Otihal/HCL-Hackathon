using HotelBooking.Core.Entities;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Exceptions;
using HotelBooking.Core.Interfaces.Repositories;
using HotelBooking.Core.Interfaces.Services;

namespace HotelBooking.Services;

public class PromotionService : IPromotionService
{
    private readonly IGenericRepository<Promotion> _promotionRepo;

    public PromotionService(IGenericRepository<Promotion> promotionRepo)
    {
        _promotionRepo = promotionRepo;
    }

    public async Task<decimal> ValidateAndApplyAsync(string code, decimal totalAmount)
    {
        var promotions = await _promotionRepo.GetAllAsync();
        var promo = promotions.FirstOrDefault(p => p.Code.ToUpper() == code.ToUpper());

        if (promo == null)
            throw new NotFoundException("Promotion code", code);

        if (!promo.IsActive)
            throw new Core.Exceptions.ValidationException("This promotion is no longer active.");

        if (promo.ExpiryDate < DateTime.UtcNow)
            throw new Core.Exceptions.ValidationException("This promotion has expired.");

        if (promo.UsageCount >= promo.UsageLimit)
            throw new Core.Exceptions.ValidationException("This promotion has reached its usage limit.");

        if (totalAmount < promo.MinBookingAmount)
            throw new Core.Exceptions.ValidationException($"Minimum booking amount of ₹{promo.MinBookingAmount} required for this promotion.");

        decimal discount;
        if (promo.DiscountType == DiscountType.Percentage)
        {
            discount = totalAmount * (promo.DiscountValue / 100);
        }
        else
        {
            discount = promo.DiscountValue;
        }

        // Increment usage
        promo.UsageCount++;
        await _promotionRepo.UpdateAsync(promo);

        return discount;
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await _promotionRepo.GetAllAsync();
    }

    public async Task<Promotion> CreateAsync(Promotion promotion)
    {
        return await _promotionRepo.AddAsync(promotion);
    }

    public async Task<Promotion> UpdateAsync(int id, Promotion updated)
    {
        var promo = await _promotionRepo.GetByIdAsync(id);
        if (promo == null)
            throw new NotFoundException("Promotion", id);

        promo.Code = updated.Code;
        promo.DiscountType = updated.DiscountType;
        promo.DiscountValue = updated.DiscountValue;
        promo.MinBookingAmount = updated.MinBookingAmount;
        promo.ExpiryDate = updated.ExpiryDate;
        promo.IsActive = updated.IsActive;
        promo.UsageLimit = updated.UsageLimit;

        await _promotionRepo.UpdateAsync(promo);
        return promo;
    }

    public async Task DeleteAsync(int id)
    {
        var promo = await _promotionRepo.GetByIdAsync(id);
        if (promo == null)
            throw new NotFoundException("Promotion", id);

        promo.IsActive = false;
        await _promotionRepo.UpdateAsync(promo);
    }
}
