using FoodOrderQuality.Core.Models;
using FoodOrderQuality.Core.Services;

namespace FoodOrderQuality.UnitTests;

public sealed class OrderPricingBoundaryTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    public void Calculate_WithQuantityBoundaryValues_UsesExpectedValidity(int quantity, bool expectedAccepted)
    {
        var service = TestServiceFactory.Create();
        var request = new OrderRequest(
            [new OrderItem("menu-1", "Durum", 100m, quantity)],
            4m,
            TestServiceFactory.OpenMondayAt18,
            null,
            "customer-1");

        var result = service.Calculate(request);

        Assert.Equal(expectedAccepted, result.Accepted);
        if (!expectedAccepted)
        {
            Assert.Contains("ITEM_QUANTITY_OUT_OF_RANGE", result.Errors);
        }
    }

    public static TheoryData<decimal, bool> MinimumSubtotalBoundaryCases => new()
    {
        { 74.99m, false },
        { OrderPricingService.MinimumDeliverySubtotal, true },
        { 75.01m, true }
    };

    [Theory]
    [MemberData(nameof(MinimumSubtotalBoundaryCases))]
    public void Calculate_WithMinimumSubtotalBoundaryValues_UsesExpectedValidity(decimal subtotal, bool expectedAccepted)
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: subtotal, distanceKm: 4m);

        var result = service.Calculate(request);

        Assert.Equal(expectedAccepted, result.Accepted);
        if (!expectedAccepted)
        {
            Assert.Contains("MINIMUM_ORDER_NOT_REACHED", result.Errors);
        }
    }

    public static TheoryData<decimal, bool, string?> DistanceBoundaryCases => new()
    {
        { -0.01m, false, "DISTANCE_NEGATIVE" },
        { 0m, true, null },
        { 10m, true, null },
        { 10.01m, false, "DISTANCE_TOO_FAR" }
    };

    [Theory]
    [MemberData(nameof(DistanceBoundaryCases))]
    public void Calculate_WithDistanceBoundaryValues_UsesExpectedValidity(decimal distanceKm, bool expectedAccepted, string? expectedError)
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, distanceKm: distanceKm);

        var result = service.Calculate(request);

        Assert.Equal(expectedAccepted, result.Accepted);
        if (expectedError is not null)
        {
            Assert.Contains(expectedError, result.Errors);
        }
    }
}