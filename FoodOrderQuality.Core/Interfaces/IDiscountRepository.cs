using FoodOrderQuality.Core.Models;

namespace FoodOrderQuality.Core.Interfaces;

public interface IDiscountRepository
{
    DiscountCode? FindByCode(string code);
}