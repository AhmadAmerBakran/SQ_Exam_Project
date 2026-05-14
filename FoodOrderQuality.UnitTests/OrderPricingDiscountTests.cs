using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;
using Moq;

namespace FoodOrderQuality.UnitTests;

public sealed class OrderPricingDiscountTests
{
    [Fact]
    public void Calculate_WithWelcome10AndEligibleSubtotal_AppliesTenPercentDiscount()
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, discountCode: "WELCOME10");

        var result = service.Calculate(request);

        Assert.True(result.Accepted);
        Assert.Equal("WELCOME10", result.AppliedDiscountCode);
        Assert.Equal(15m, result.DiscountAmount);
        Assert.Equal(174m, result.Total);
    }

    [Fact]
    public void Calculate_WithFreeShipAndEligibleSubtotal_RemovesDeliveryFee()
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 220m, distanceKm: 4m, discountCode: "FREESHIP");

        var result = service.Calculate(request);

        Assert.True(result.Accepted);
        Assert.Equal("FREESHIP", result.AppliedDiscountCode);
        Assert.Equal(0m, result.DeliveryFee);
        Assert.Equal(220m, result.Total);
    }

    [Fact]
    public void Calculate_WithUnknownDiscountCode_RejectsOrder()
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, discountCode: "NOPE");

        var result = service.Calculate(request);

        Assert.False(result.Accepted);
        Assert.Contains("DISCOUNT_CODE_UNKNOWN", result.Errors);
    }

    [Fact]
    public void Calculate_WithExpiredDiscountCode_RejectsOrder()
    {
        var repository = new Mock<IDiscountRepository>();
        repository
            .Setup(repo => repo.FindByCode("OLD10"))
            .Returns(new DiscountCode("OLD10", DiscountKind.Percentage, 10m, 100m, new DateOnly(2026, 5, 1)));

        var service = TestServiceFactory.Create(discountRepository: repository.Object);
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, discountCode: "OLD10");

        var result = service.Calculate(request);

        Assert.False(result.Accepted);
        Assert.Contains("DISCOUNT_CODE_EXPIRED", result.Errors);
    }

    [Fact]
    public void Calculate_WithDiscountBelowRequiredSubtotal_RejectsOrder()
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 99m, discountCode: "WELCOME10");

        var result = service.Calculate(request);

        Assert.False(result.Accepted);
        Assert.Contains("DISCOUNT_MINIMUM_NOT_REACHED", result.Errors);
    }
}