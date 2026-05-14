namespace FoodOrderQuality.Api.Models;

public enum DiscountKind
{
    Percentage,
    FreeDelivery,
    FixedAmount
}

public sealed record DiscountCode(
    
    string Code,
    DiscountKind Kind,
    decimal Value,
    decimal MinimumSubtotal,
    DateOnly? ExpiresOn = null );
