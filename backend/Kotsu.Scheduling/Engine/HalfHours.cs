using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Engine;

/// <summary>
/// Converts between study hours and whole half-hours, the unit the engine shares and places time in.
/// Study hours are always in half-hour steps, so the conversion is exact both ways.
/// </summary>
internal static class HalfHours
{
    public static int Of(StudyHours hours) => (int)(hours.Value * 2);

    public static StudyHours ToHours(int halfHours) => StudyHours.Create(halfHours * 0.5m).Value;
}
