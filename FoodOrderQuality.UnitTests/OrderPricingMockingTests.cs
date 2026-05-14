using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;
using Moq;

namespace FoodOrderQuality.UnitTests;

public sealed class OrderPricingMockingTests
{
    [Fact]
    public void Calculate_WhenRequestHasNoTime_UsesClockDependency()
    {
        var service = TestServiceFactory.Create(clockNow: TestServiceFactory.ClosedSundayAt18);
        var request = TestServiceFactory.ValidRequest(requestedTime: null) with { RequestedDeliveryTime = null };

        var result = service.Calculate(request);

        Assert.False(result.Accepted);
        Assert.Contains("OUTSIDE_OPENING_HOURS", result.Errors);
    }

    [Fact]
    public void Calculate_WithDiscountCode_UsesRepositoryDependency()
    {
        var repository = new Mock<IDiscountRepository>();
        repository
            .Setup(repo => repo.FindByCode("TAKE25"))
            .Returns(new DiscountCode("TAKE25", DiscountKind.FixedAmount, 25m, 150m));

        var service = TestServiceFactory.Create(discountRepository: repository.Object);
        var request = TestServiceFactory.ValidRequest(subtotal: 150m, discountCode: "TAKE25");

        var result = service.Calculate(request);

        Assert.True(result.Accepted);
        Assert.Equal(25m, result.DiscountAmount);
        repository.Verify(repo => repo.FindByCode("TAKE25"), Times.Once);
    }
}