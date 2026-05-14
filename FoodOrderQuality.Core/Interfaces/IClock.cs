namespace FoodOrderQuality.Core.Interfaces;

public interface IClock
{
    DateTimeOffset Now { get; }
}