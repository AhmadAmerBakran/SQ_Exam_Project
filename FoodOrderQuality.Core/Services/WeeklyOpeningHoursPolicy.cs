using FoodOrderQuality.Core.Interfaces;

namespace FoodOrderQuality.Core.Services;

public sealed class WeeklyOpeningHoursPolicy : IOpeningHoursPolicy
{
    public bool IsOpenAt(DateTimeOffset time)
    {
        var localTime = time.TimeOfDay;

        return time.DayOfWeek switch
        {
            DayOfWeek.Sunday => false,
            DayOfWeek.Friday or DayOfWeek.Saturday =>
                localTime >= new TimeSpan(16, 0, 0) && localTime <= new TimeSpan(23, 30, 0),
            _ =>
                localTime >= new TimeSpan(16, 0, 0) && localTime <= new TimeSpan(22, 0, 0)
        };
    }
}