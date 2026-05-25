using HotelBooking.Core.Entities;

namespace HotelBooking.Core.Interfaces.Services;

public interface IPromotionService
{
    Task<decimal> ValidateAndApplyAsync(string code, decimal totalAmount);
    Task<IEnumerable<Promotion>> GetAllAsync();
    Task<Promotion> CreateAsync(Promotion promotion);
    Task<Promotion> UpdateAsync(int id, Promotion promotion);
    Task DeleteAsync(int id);
}
