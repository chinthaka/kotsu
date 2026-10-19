namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The calendar dates a schedule covers: 1 to 4 whole weeks from a start date.
/// </summary>
public sealed record ScheduleWindow
{
    public const int MinWeeks = 1;
    public const int MaxWeeks = 4;

    private ScheduleWindow(DateOnly startDate, int weeks)
    {
        StartDate = startDate;
        Weeks = weeks;
    }

    public DateOnly StartDate { get; }

    public int Weeks { get; }

    /// <summary>
    /// The last date in the window, inclusive.
    /// </summary>
    public DateOnly EndDate => StartDate.AddDays((7 * Weeks) - 1);

    public static Result<ScheduleWindow> Create(DateOnly startDate, int weeks)
    {
        var errors = new List<ValidationError>();
        if (weeks is < MinWeeks or > MaxWeeks)
        {
            errors.Add(new("schedule-window.weeks-out-of-range", $"A schedule covers {MinWeeks} to {MaxWeeks} weeks."));
        }

        return Result.From(errors, () => new ScheduleWindow(startDate, weeks));
    }
}
