using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;
using FoodOrderQuality.Core.Services;
using Moq;

namespace FoodOrderQuality.UnitTests;

internal static class TestServiceFactory
{
    internal static readonly DateTimeOffset OpenMondayAt18 = new(2026, 5, 18, 18, 0, 0, TimeSpan.Zero);
    internal static readonly DateTimeOffset ClosedSundayAt18 = new(2026, 5, 17, 18, 0, 0, TimeSpan.Zero);

    internal static OrderPricingService Create(
        IDiscountRepository? discountRepository = null,
        IOpeningHoursPolicy? openingHoursPolicy = null,
        DateTimeOffset? clockNow = null)
    {
        var clockMock = new Mock<IClock>();
        clockMock.Setup(clock => clock.Now).Returns(clockNow ?? OpenMondayAt18);

        return new OrderPricingService(
            new DeliveryFeeCalculator(),
            discountRepository ?? new InMemoryDiscountRepository(),
            openingHoursPolicy ?? new WeeklyOpeningHoursPolicy(),
            clockMock.Object);
    }

    internal static OrderRequest ValidRequest(
        decimal subtotal = 150m,
        decimal distanceKm = 4m,
        string? discountCode = null,
        DateTimeOffset? requestedTime = null)
    {
        return new OrderRequest(
            Items: [new OrderItem("menu-1", "Chicken Durum", subtotal, 1)],
            DeliveryDistanceKm: distanceKm,
            RequestedDeliveryTime: requestedTime ?? OpenMondayAt18,
            DiscountCode: discountCode,
            CustomerId: "customer-1");
    }
}