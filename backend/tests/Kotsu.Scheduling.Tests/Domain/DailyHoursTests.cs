using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class DailyHoursTests
{
    private static StudyHours Hours(decimal hours) => StudyHours.Create(hours).Value;

    [Fact]
    public void DailyHours_WithTheSameDateAndHours_AreEqual()
    {
        // Arrange
        var date = new DateOnly(2026, 11, 2);
        var first = new DailyHours(date, Hours(1.5m));

        // Act
        var second = new DailyHours(date, Hours(1.5m));

        // Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void DailyHours_WithDifferentHours_AreNotEqual()
    {
        // Arrange
        var date = new DateOnly(2026, 11, 2);
        var first = new DailyHours(date, Hours(1.5m));

        // Act
        var second = new DailyHours(date, Hours(2m));

        // Assert
        Assert.NotEqual(first, second);
    }
}
