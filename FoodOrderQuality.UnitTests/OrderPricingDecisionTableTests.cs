using FoodOrderQuality.Core.Models;

namespace FoodOrderQuality.UnitTests;

public sealed class OrderPricingDecisionTableTests
{
    public static IEnumerable<object[]> InvalidOrderRules()
    {
        yield return [Array.Empty<OrderItem>(), 0m, 4m, TestServiceFactory.OpenMondayAt18, "BASKET_EMPTY"];
        yield return [new[] { new OrderItem("menu-1", "Durum", 74m, 1) }, 74m, 4m, TestServiceFactory.OpenMondayAt18, "MINIMUM_ORDER_NOT_REACHED"];
        yield return [new[] { new OrderItem("menu-1", "Durum", 150m, 1) }, 150m, 10.01m, TestServiceFactory.OpenMondayAt18, "DISTANCE_TOO_FAR"];
        yield return [new[] { new OrderItem("menu-1", "Durum", 150m, 1) }, 150m, 4m, TestServiceFactory.ClosedSundayAt18, "OUTSIDE_OPENING_HOURS"];
    }

    [Theory]
    [MemberData(nameof(InvalidOrderRules))]
    public void Calculate_WhenOneDecisionTableConditionFails_RejectsOrder(
        IReadOnlyCollection<OrderItem> items,
        decimal subtotal,
        decimal distanceKm,
        DateTimeOffset requestedTime,
        string expectedError)
    {
        var service = TestServiceFactory.Create();
        var request = new OrderRequest(items, distanceKm, requestedTime, null, "customer-1");

        var result = service.Calculate(request);

        Assert.False(result.Accepted);
        Assert.Contains(expectedError, result.Errors);
        Assert.Equal(subtotal, result.Subtotal);
    }

    [Fact]
    public void Calculate_WhenAllDecisionTableConditionsAreTrue_AcceptsOrder()
    {
        var service = TestServiceFactory.Create();
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, distanceKm: 4m);

        var result = service.Calculate(request);

        Assert.True(result.Accepted);
        Assert.Empty(result.Errors);
        Assert.Equal(150m, result.Subtotal);
        Assert.Equal(39m, result.DeliveryFee);
        Assert.Equal(189m, result.Total);
    }
}