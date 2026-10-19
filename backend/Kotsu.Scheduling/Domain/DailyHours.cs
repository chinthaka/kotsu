namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The study hours available on one date.
/// </summary>
public readonly record struct DailyHours(DateOnly Date, StudyHours Hours);
