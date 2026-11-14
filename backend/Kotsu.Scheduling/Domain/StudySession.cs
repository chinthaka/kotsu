namespace Kotsu.Scheduling.Domain;

/// <summary>
/// One sub-topic studied on one date, for the hours given to it that day.
/// </summary>
public readonly record struct StudySession(DateOnly Date, SubjectId SubjectId, SubTopicId SubTopicId, StudyHours Hours);
