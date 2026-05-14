using FoodOrderQuality.Core.Services;

namespace FoodOrderQuality.UnitTests;

public sealed class OpeningHoursPolicyTests
{
    [Theory]
    [InlineData("2026-05-18T15:59:00+00:00", false)]
    [InlineData("2026-05-18T16:00:00+00:00", true)]
    [InlineData("2026-05-18T22:00:00+00:00", true)]
    [InlineData("2026-05-18T22:01:00+00:00", false)]
    [InlineData("2026-05-22T23:30:00+00:00", true)]
    [InlineData("2026-05-22T23:31:00+00:00", false)]
    [InlineData("2026-05-17T18:00:00+00:00", false)]
    public void IsOpenAt_WithOpeningHourBoundaryValues_ReturnsExpectedResult(string timeText, bool expected)
    {
        var policy = new WeeklyOpeningHoursPolicy();
        var time = DateTimeOffset.Parse(timeText);

        var isOpen = policy.IsOpenAt(time);

        Assert.Equal(expected, isOpen);
    }
}