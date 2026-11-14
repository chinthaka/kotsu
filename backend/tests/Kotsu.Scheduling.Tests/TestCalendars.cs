using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests;

/// <summary>
/// Builds study calendars for tests. The default is phase 3's example week from Monday 2 November 2026.
/// </summary>
internal static class TestCalendars
{
    public static readonly DateOnly Monday = new(2026, 11, 2);

    /// <summary>
    /// Monday to Thursday 2 hours, Friday 1.5, Saturday 4, Sunday 0: 13.5 hours a week.
    /// </summary>
    public static WeeklyAvailability ExampleWeek() => Week(2m, 2m, 2m, 2m, 1.5m, 4m, 0m);

    public static WeeklyAvailability EveryDay(decimal hours) =>
        WeeklyAvailability.Create(IsoWeekdays.All.ToDictionary(day => day, _ => TestContent.Hours(hours))).Value;

    /// <summary>
    /// Hours for each day, Monday first.
    /// </summary>
    public static WeeklyAvailability Week(
        decimal monday, decimal tuesday, decimal wednesday, decimal thursday, decimal friday, decimal saturday, decimal sunday) =>
        WeeklyAvailability.Create(new Dictionary<DayOfWeek, StudyHours>
        {
            [DayOfWeek.Monday] = TestContent.Hours(monday),
            [DayOfWeek.Tuesday] = TestContent.Hours(tuesday),
            [DayOfWeek.Wednesday] = TestContent.Hours(wednesday),
            [DayOfWeek.Thursday] = TestContent.Hours(thursday),
            [DayOfWeek.Friday] = TestContent.Hours(friday),
            [DayOfWeek.Saturday] = TestContent.Hours(saturday),
            [DayOfWeek.Sunday] = TestContent.Hours(sunday),
        }).Value;

    public static StudyCalendar Calendar(DateOnly start, int weeks, WeeklyAvailability availability) =>
        StudyCalendar.Create(ScheduleWindow.Create(start, weeks).Value, availability).Value;

    /// <summary>
    /// One week of the example week from Monday 2 November 2026: 13.5 hours, or 27 half-hours.
    /// </summary>
    public static StudyCalendar ExampleCalendar() => Calendar(Monday, 1, ExampleWeek());
}
