namespace FoodOrderQuality.Core.Interfaces;

public interface IOpeningHoursPolicy
{
    bool IsOpenAt(DateTimeOffset time);
}