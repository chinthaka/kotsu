using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class IsoWeekdaysTests
{
    [Fact]
    public void All_HasEveryDayOfTheWeekOnce()
    {
        // Act
        var days = IsoWeekdays.All;

        // Assert
        Assert.Equal(7, days.Count);
        Assert.Equal(7, days.Distinct().Count());
    }

    [Fact]
    public void All_RunsMondayToSunday()
    {
        // Act
        var days = IsoWeekdays.All;

        // Assert
        Assert.Equal(
            [
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday,
            ],
            days);
    }
}
