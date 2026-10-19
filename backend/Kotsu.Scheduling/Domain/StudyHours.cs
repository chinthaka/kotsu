namespace Kotsu.Scheduling.Domain;

/// <summary>
/// An amount of study time in hours: zero or more, in half-hour steps.
/// </summary>
public readonly record struct StudyHours : IComparable<StudyHours>
{
    private const decimal Step = 0.5m;

    private StudyHours(decimal value) => Value = value;

    public static StudyHours Zero { get; } = new(0m);

    public decimal Value { get; }

    public static Result<StudyHours> Create(decimal hours)
    {
        var errors = new List<ValidationError>();
        if (hours < 0)
        {
            errors.Add(new("study-hours.negative", "Study hours can't be negative."));
        }

        if (hours % Step != 0)
        {
            errors.Add(new("study-hours.not-half-hour-step", "Study hours must be in half-hour steps."));
        }

        return Result.From(errors, () => new StudyHours(hours));
    }

    public static StudyHours operator +(StudyHours left, StudyHours right) => new(left.Value + right.Value);

    public static bool operator <(StudyHours left, StudyHours right) => left.Value < right.Value;

    public static bool operator >(StudyHours left, StudyHours right) => left.Value > right.Value;

    public static bool operator <=(StudyHours left, StudyHours right) => left.Value <= right.Value;

    public static bool operator >=(StudyHours left, StudyHours right) => left.Value >= right.Value;

    public int CompareTo(StudyHours other) => Value.CompareTo(other.Value);

    public override string ToString() => $"{Value}h";
}
