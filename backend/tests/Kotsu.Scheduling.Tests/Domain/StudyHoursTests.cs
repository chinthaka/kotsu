using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class StudyHoursTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(1)]
    [InlineData(1.5)]
    [InlineData(40)]
    public void Create_WithHalfHourStep_Succeeds(double hours)
    {
        // Arrange
        var value = (decimal)hours;

        // Act
        var result = StudyHours.Create(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
    }

    [Fact]
    public void Create_WithNegativeHours_FailsWithNegative()
    {
        // Act
        var result = StudyHours.Create(-1m);

        // Assert
        Assert.Equal(["study-hours.negative"], result.Errors.Select(e => e.Code));
    }

    [Theory]
    [InlineData(1.25)]
    [InlineData(0.1)]
    [InlineData(2.75)]
    public void Create_OffHalfHourStep_FailsWithNotHalfHourStep(double hours)
    {
        // Act
        var result = StudyHours.Create((decimal)hours);

        // Assert
        Assert.Equal(["study-hours.not-half-hour-step"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_NegativeAndOffStep_ReportsBoth()
    {
        // Act
        var result = StudyHours.Create(-0.25m);

        // Assert
        Assert.Equal(["study-hours.negative", "study-hours.not-half-hour-step"], result.Errors.Select(e => e.Code).Order());
    }

    [Fact]
    public void Add_TwoAmounts_ReturnsTheirSum()
    {
        // Arrange
        var first = StudyHours.Create(1.5m).Value;
        var second = StudyHours.Create(2m).Value;

        // Act
        var sum = first + second;

        // Assert
        Assert.Equal(3.5m, sum.Value);
    }

    [Fact]
    public void Zero_HasNoHours()
    {
        // Act
        var zero = StudyHours.Zero;

        // Assert
        Assert.Equal(0m, zero.Value);
    }
}
