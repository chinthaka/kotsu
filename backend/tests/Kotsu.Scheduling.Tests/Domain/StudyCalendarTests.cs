using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class StudyCalendarTests
{
    private static readonly DateOnly _monday = new(2026, 11, 2);

    private static StudyHours Hours(decimal hours) => StudyHours.Create(hours).Value;

    private static ScheduleWindow Window(DateOnly start, int weeks) => ScheduleWindow.Create(start, weeks).Value;

    /// <summary>
    /// Monday to Thursday 2 hours, Friday 1.5, Saturday 4, Sunday 0: 13.5 hours a week.
    /// </summary>
    private static WeeklyAvailability ExampleWeek() =>
        WeeklyAvailability.Create(new Dictionary<DayOfWeek, StudyHours>
        {
            [DayOfWeek.Monday] = Hours(2m),
            [DayOfWeek.Tuesday] = Hours(2m),
            [DayOfWeek.Wednesday] = Hours(2m),
            [DayOfWeek.Thursday] = Hours(2m),
            [DayOfWeek.Friday] = Hours(1.5m),
            [DayOfWeek.Saturday] = Hours(4m),
            [DayOfWeek.Sunday] = Hours(0m),
        }).Value;

    private static WeeklyAvailability EveryDay(decimal hours) =>
        WeeklyAvailability.Create(IsoWeekdays.All.ToDictionary(day => day, _ => Hours(hours))).Value;

    private static StudyCalendar Calendar(DateOnly start, int weeks, WeeklyAvailability availability) =>
        StudyCalendar.Create(Window(start, weeks), availability).Value;

    [Fact]
    public void Create_ForOneWeek_HasEveryDateInOrder()
    {
        // Arrange
        var window = Window(_monday, 1);

        // Act
        var result = StudyCalendar.Create(window, ExampleWeek());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            [
                new DateOnly(2026, 11, 2),
                new DateOnly(2026, 11, 3),
                new DateOnly(2026, 11, 4),
                new DateOnly(2026, 11, 5),
                new DateOnly(2026, 11, 6),
                new DateOnly(2026, 11, 7),
                new DateOnly(2026, 11, 8),
            ],
            result.Value.Days.Select(d => d.Date));
    }

    [Theory]
    [InlineData(2, 2.0)] // Monday
    [InlineData(6, 1.5)] // Friday
    [InlineData(7, 4.0)] // Saturday
    [InlineData(8, 0.0)] // Sunday
    public void Create_GivesEachDateTheHoursForItsWeekday(int dayOfNovember, double expectedHours)
    {
        // Arrange
        var date = new DateOnly(2026, 11, dayOfNovember);

        // Act
        var calendar = Calendar(_monday, 1, ExampleWeek());

        // Assert
        Assert.Contains(new DailyHours(date, Hours((decimal)expectedHours)), calendar.Days);
    }

    [Fact]
    public void Create_KeepsTheWindow()
    {
        // Arrange
        var window = Window(_monday, 2);

        // Act
        var calendar = StudyCalendar.Create(window, ExampleWeek()).Value;

        // Assert
        Assert.Equal(window, calendar.Window);
    }

    [Fact]
    public void HoursOn_ADateInTheWindow_IsThatDatesHours()
    {
        // Arrange
        var calendar = Calendar(_monday, 1, ExampleWeek());

        // Act
        var hours = calendar.HoursOn(new DateOnly(2026, 11, 6));

        // Assert
        Assert.Equal(Hours(1.5m), hours);
    }

    [Fact]
    public void HoursOn_TheDayBeforeTheWindow_IsZero()
    {
        // Arrange
        var calendar = Calendar(_monday, 1, EveryDay(3m));

        // Act
        var hours = calendar.HoursOn(_monday.AddDays(-1));

        // Assert
        Assert.Equal(StudyHours.Zero, hours);
    }

    [Fact]
    public void HoursOn_TheDayAfterTheWindow_IsZero()
    {
        // Arrange
        var calendar = Calendar(_monday, 1, EveryDay(3m));

        // Act
        var hours = calendar.HoursOn(calendar.Window.EndDate.AddDays(1));

        // Assert
        Assert.Equal(StudyHours.Zero, hours);
    }

    [Fact]
    public void TotalHours_ForTheExampleWeek_Is13AndAHalf()
    {
        // Arrange
        var calendar = Calendar(_monday, 1, ExampleWeek());

        // Act
        var total = calendar.TotalHours;

        // Assert
        Assert.Equal(13.5m, total.Value);
    }
}
