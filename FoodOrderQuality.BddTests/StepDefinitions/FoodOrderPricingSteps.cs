using FoodOrderQuality.Core.Models;
using FoodOrderQuality.Core.Services;
using Reqnroll;

namespace FoodOrderQuality.BddTests.StepDefinitions;

[Binding]
public sealed class FoodOrderPricingSteps
{
    private decimal _subtotal;
    private decimal _distanceKm;
    private DateTimeOffset _deliveryTime;
    private string? _discountCode;
    private OrderResult? _result;

    [Given(@"the customer has an order subtotal of (.*) DKK")]
    public void GivenTheCustomerHasAnOrderSubtotalOfDkk(decimal subtotal)
    {
        _subtotal = subtotal;
    }

    [Given(@"the delivery distance is (.*) km")]
    public void GivenTheDeliveryDistanceIsKm(decimal distanceKm)
    {
        _distanceKm = distanceKm;
    }

    [Given(@"the delivery time is ""(.*)""")]
    public void GivenTheDeliveryTimeIs(string deliveryTimeText)
    {
        _deliveryTime = DateTimeOffset.Parse(deliveryTimeText);
    }

    [Given(@"the discount code is ""(.*)""")]
    public void GivenTheDiscountCodeIs(string discountCode)
    {
        _discountCode = discountCode;
    }

    [When(@"the order is calculated")]
    public void WhenTheOrderIsCalculated()
    {
        var service = new OrderPricingService(
            new DeliveryFeeCalculator(),
            new InMemoryDiscountRepository(),
            new WeeklyOpeningHoursPolicy(),
            new SystemClock());

        var request = new OrderRequest(
            [new OrderItem("menu-1", "Chicken Durum", _subtotal, 1)],
            _distanceKm,
            _deliveryTime,
            _discountCode,
            "bdd-customer");

        _result = service.Calculate(request);
    }

    [Then(@"the order should be accepted")]
    public void ThenTheOrderShouldBeAccepted()
    {
        Assert.NotNull(_result);
        Assert.True(_result.Accepted, string.Join(", ", _result.Errors));
    }

    [Then(@"the order should be rejected with error ""(.*)""")]
    public void ThenTheOrderShouldBeRejectedWithError(string expectedError)
    {
        Assert.NotNull(_result);
        Assert.False(_result.Accepted);
        Assert.Contains(expectedError, _result.Errors);
    }

    [Then(@"the delivery fee should be (.*) DKK")]
    public void ThenTheDeliveryFeeShouldBeDkk(decimal expectedDeliveryFee)
    {
        Assert.NotNull(_result);
        Assert.Equal(expectedDeliveryFee, _result.DeliveryFee);
    }

    [Then(@"the total should be (.*) DKK")]
    public void ThenTheTotalShouldBeDkk(decimal expectedTotal)
    {
        Assert.NotNull(_result);
        Assert.Equal(expectedTotal, _result.Total);
    }
}