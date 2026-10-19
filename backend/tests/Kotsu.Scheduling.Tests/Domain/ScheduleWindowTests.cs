using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class ScheduleWindowTests
{
    private static readonly DateOnly _monday = new(2026, 11, 2);

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Create_WithOneToFourWeeks_Succeeds(int weeks)
    {
        // Act
        var result = ScheduleWindow.Create(_monday, weeks);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_monday, result.Value.StartDate);
        Assert.Equal(weeks, result.Value.Weeks);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void Create_OutsideOneToFourWeeks_FailsWithWeeksOutOfRange(int weeks)
    {
        // Act
        var result = ScheduleWindow.Create(_monday, weeks);

        // Assert
        Assert.Equal(["schedule-window.weeks-out-of-range"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void EndDate_ForOneWeekFromMonday_IsTheFollowingSunday()
    {
        // Arrange
        var window = ScheduleWindow.Create(_monday, 1).Value;

        // Act
        var endDate = window.EndDate;

        // Assert
        Assert.Equal(new DateOnly(2026, 11, 8), endDate);
    }

    [Fact]
    public void EndDate_AcrossMonthBoundary_IsInclusive()
    {
        // Arrange
        var window = ScheduleWindow.Create(new DateOnly(2026, 10, 26), 2).Value;

        // Act
        var endDate = window.EndDate;

        // Assert
        Assert.Equal(new DateOnly(2026, 11, 8), endDate);
    }

    [Fact]
    public void EndDate_AcrossYearBoundary_IsInclusive()
    {
        // Arrange
        var window = ScheduleWindow.Create(new DateOnly(2026, 12, 28), 4).Value;

        // Act
        var endDate = window.EndDate;

        // Assert
        Assert.Equal(new DateOnly(2027, 1, 24), endDate);
    }
}
