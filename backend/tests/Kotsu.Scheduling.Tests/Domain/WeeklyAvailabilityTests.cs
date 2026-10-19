using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class WeeklyAvailabilityTests
{
    private static StudyHours Hours(decimal hours) => StudyHours.Create(hours).Value;

    private static Dictionary<DayOfWeek, StudyHours> EveryDay(decimal hours) =>
        Enum.GetValues<DayOfWeek>().ToDictionary(day => day, _ => Hours(hours));

    [Fact]
    public void Create_WithHoursForEveryDay_Succeeds()
    {
        // Arrange
        var hours = EveryDay(2m);
        hours[DayOfWeek.Saturday] = Hours(4.5m);

        // Act
        var result = WeeklyAvailability.Create(hours);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4.5m, result.Value.HoursOn(DayOfWeek.Saturday).Value);
        Assert.Equal(2m, result.Value.HoursOn(DayOfWeek.Monday).Value);
    }

    [Fact]
    public void Create_WithZeroHoursEveryDay_Succeeds()
    {
        // Act
        var result = WeeklyAvailability.Create(EveryDay(0m));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StudyHours.Zero, result.Value.TotalHours);
    }

    [Fact]
    public void Create_WithAMissingDay_FailsWithMissingDay()
    {
        // Arrange
        var hours = EveryDay(1m);
        hours.Remove(DayOfWeek.Wednesday);

        // Act
        var result = WeeklyAvailability.Create(hours);

        // Assert
        Assert.Equal(["weekly-availability.missing-day"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_With24HoursOnADay_Succeeds()
    {
        // Arrange
        var hours = EveryDay(1m);
        hours[DayOfWeek.Sunday] = Hours(24m);

        // Act
        var result = WeeklyAvailability.Create(hours);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithMoreThan24HoursOnADay_FailsWithTooManyHours()
    {
        // Arrange
        var hours = EveryDay(1m);
        hours[DayOfWeek.Sunday] = Hours(24.5m);

        // Act
        var result = WeeklyAvailability.Create(hours);

        // Assert
        Assert.Equal(["weekly-availability.too-many-hours"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithAnUnknownDay_FailsWithUnknownDay()
    {
        // Arrange
        var hours = EveryDay(1m);
        hours[(DayOfWeek)9] = Hours(1m);

        // Act
        var result = WeeklyAvailability.Create(hours);

        // Assert
        Assert.Equal(["weekly-availability.unknown-day"], result.Errors.Select(e => e.Code));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1.25)]
    public void DayHours_ThatAreNegativeOrOffStep_CannotBeCreated(double hours)
    {
        // Act
        var result = StudyHours.Create((decimal)hours);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void TotalHours_IsTheSumOfEveryDay()
    {
        // Arrange
        var availability = WeeklyAvailability.Create(EveryDay(1.5m)).Value;

        // Act
        var total = availability.TotalHours;

        // Assert
        Assert.Equal(10.5m, total.Value);
    }
}
