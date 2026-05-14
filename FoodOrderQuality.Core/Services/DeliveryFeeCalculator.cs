namespace FoodOrderQuality.Core.Services;

public sealed class DeliveryFeeCalculator
{ 
    public decimal Calculate(decimal deliveryDistanceKm)
    {
        if (deliveryDistanceKm < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(deliveryDistanceKm), "Distance cannot be negative.");
        }

        if (deliveryDistanceKm > 10m)
        {
            throw new ArgumentOutOfRangeException(nameof(deliveryDistanceKm), "Delivery distance cannot be above 10 km.");
        }

        if (deliveryDistanceKm <= 2m)
        {
            return 29m;
        }

        if (deliveryDistanceKm <= 5m)
        {
            return 39m;
        }

        return 59m;
    }
}