namespace FoodOrderQuality.Api.Models;

public sealed record OrderResult(
    
    bool Accepted,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal DiscountAmount,
    decimal Total,
    string? AppliedDiscountCode,
    IReadOnlyCollection<string> Errors)
{
    public static OrderResult Rejected(
        decimal subtotal,
        decimal deliveryFee,
        decimal discountAmount,
        IEnumerable<string> errors) =>
        new(false, subtotal, deliveryFee, discountAmount, 0m, null, errors.ToArray());
}