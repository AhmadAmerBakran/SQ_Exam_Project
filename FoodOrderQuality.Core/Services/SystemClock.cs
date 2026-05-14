using FoodOrderQuality.Core.Interfaces;

namespace FoodOrderQuality.Core.Services;

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}