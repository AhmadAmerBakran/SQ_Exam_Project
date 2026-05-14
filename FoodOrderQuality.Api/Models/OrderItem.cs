namespace FoodOrderQuality.Api.Models;

public sealed record OrderItem(
    
    string MenuItemId,
    string Name,
    decimal UnitPrice,
    int Quantity );