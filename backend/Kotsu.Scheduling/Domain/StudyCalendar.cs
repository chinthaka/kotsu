namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The study hours available on each date of a schedule window, taken from the
/// student's weekly hours. Every date is included, even one with zero hours.
/// </summary>
public sealed class StudyCalendar
{
    private StudyCalendar(ScheduleWindow window, IReadOnlyList<DailyHours> days)
    {
        Window = window;
        Days = days;
        TotalHours = days.Aggregate(StudyHours.Zero, (total, day) => total + day.Hours);
    }

    public ScheduleWindow Window { get; }

    /// <summary>
    /// One entry for every date from the window's start to its end, in date order.
    /// </summary>
    public IReadOnlyList<DailyHours> Days { get; }

    public StudyHours TotalHours { get; }

    /// <summary>
    /// The hours on <paramref name="date"/>, or zero for a date outside the window.
    /// </summary>
    public StudyHours HoursOn(DateOnly date) =>
        date < Window.StartDate || date > Window.EndDate
            ? StudyHours.Zero
            : Days[date.DayNumber - Window.StartDate.DayNumber].Hours;

    /// <summary>
    /// Always succeeds: the window and the weekly hours are already valid.
    /// </summary>
    public static Result<StudyCalendar> Create(ScheduleWindow window, WeeklyAvailability availability)
    {
        var days = Enumerable.Range(0, 7 * window.Weeks)
            .Select(offset => window.StartDate.AddDays(offset))
            .Select(date => new DailyHours(date, availability.HoursOn(date.DayOfWeek)))
            .ToList()
            .AsReadOnly();

        return Result.Success(new StudyCalendar(window, days));
    }
}
