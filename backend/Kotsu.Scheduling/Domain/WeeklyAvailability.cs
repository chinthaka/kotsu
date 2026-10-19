namespace Kotsu.Scheduling.Domain;

/// <summary>
/// How many hours a student can study on each of the 7 days of the week.
/// </summary>
public sealed class WeeklyAvailability
{
    private static readonly StudyHours _maxHoursPerDay = StudyHours.Create(24m).Value;

    private readonly IReadOnlyDictionary<DayOfWeek, StudyHours> _hours;

    private WeeklyAvailability(IReadOnlyDictionary<DayOfWeek, StudyHours> hours)
    {
        _hours = hours;
        TotalHours = hours.Values.Aggregate(StudyHours.Zero, (total, h) => total + h);
    }

    public StudyHours TotalHours { get; }

    public StudyHours HoursOn(DayOfWeek day) => _hours[day];

    public static Result<WeeklyAvailability> Create(IReadOnlyDictionary<DayOfWeek, StudyHours> hours)
    {
        var errors = new List<ValidationError>();

        var missing = Enum.GetValues<DayOfWeek>().Where(day => !hours.ContainsKey(day)).ToList();
        if (missing.Count > 0)
        {
            errors.Add(new("weekly-availability.missing-day", $"Hours are missing for {string.Join(", ", missing)}."));
        }

        if (hours.Keys.Any(day => !Enum.IsDefined(day)))
        {
            errors.Add(new("weekly-availability.unknown-day", "Hours were given for a day that isn't a day of the week."));
        }

        if (hours.Values.Any(h => h > _maxHoursPerDay))
        {
            errors.Add(new("weekly-availability.too-many-hours", "A day can have at most 24 hours of study."));
        }

        return Result.From(errors, () => new WeeklyAvailability(hours.ToDictionary().AsReadOnly()));
    }
}
