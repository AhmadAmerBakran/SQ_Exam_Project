using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;

namespace FoodOrderQuality.Core.Services;

public sealed class InMemoryDiscountRepository : IDiscountRepository
{
    private readonly Dictionary<string, DiscountCode> _codes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["WELCOME10"] = new("WELCOME10", DiscountKind.Percentage, 10m, 100m),
        ["FREESHIP"] = new("FREESHIP", DiscountKind.FreeDelivery, 0m, 200m),
        ["TAKE25"] = new("TAKE25", DiscountKind.FixedAmount, 25m, 150m)
    };

    public DiscountCode? FindByCode(string code)
    {
        return _codes.TryGetValue(code.Trim(), out var discountCode)
            ? discountCode
            : null;
    }
}