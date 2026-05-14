using FoodOrderQuality.Core.Services;

namespace FoodOrderQuality.UnitTests;

public sealed class DeliveryFeeCalculatorTests
{
    [Theory]
    [InlineData(0, 29)]
    [InlineData(2, 29)]
    [InlineData(2.01, 39)]
    [InlineData(5, 39)]
    [InlineData(5.01, 59)]
    [InlineData(10, 59)]
    public void Calculate_WithBoundaryDistances_ReturnsExpectedFee(decimal distanceKm, decimal expectedFee)
    {
        var calculator = new DeliveryFeeCalculator();

        var fee = calculator.Calculate(distanceKm);

        Assert.Equal(expectedFee, fee);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(10.01)]
    public void Calculate_WithInvalidDistance_Throws(decimal distanceKm)
    {
        var calculator = new DeliveryFeeCalculator();

        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.Calculate(distanceKm));
    }
}