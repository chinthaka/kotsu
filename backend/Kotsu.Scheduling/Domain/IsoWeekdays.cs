namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The days of the week in ISO-8601 order, Monday to Sunday. Use this rather than
/// <c>Enum.GetValues&lt;DayOfWeek&gt;()</c>, which starts on Sunday.
/// </summary>
public static class IsoWeekdays
{
    public static IReadOnlyList<DayOfWeek> All { get; } =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday,
    ];
}
