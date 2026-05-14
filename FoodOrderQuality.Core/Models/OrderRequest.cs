namespace FoodOrderQuality.Core.Models;

public sealed record OrderRequest(
    
    IReadOnlyCollection<OrderItem>? Items,
    decimal DeliveryDistanceKm,
    DateTimeOffset? RequestedDeliveryTime,
    string? DiscountCode,
    string? CustomerId = null);