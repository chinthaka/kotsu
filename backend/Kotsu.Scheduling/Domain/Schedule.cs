namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A study schedule: the sessions placed on each date of a window, the hours each
/// subject was given and the hours left unused. Only the scheduling engine makes one.
/// </summary>
public sealed class Schedule
{
    internal Schedule(ScheduleWindow window, IReadOnlyList<StudySession> sessions, StudyHours unusedHours)
    {
        Window = window;
        Sessions = sessions;
        UnusedHours = unusedHours;
    }

    public ScheduleWindow Window { get; }

    /// <summary>
    /// In date order, then by subject in the order the subjects were given, then in syllabus order.
    /// </summary>
    public IReadOnlyList<StudySession> Sessions { get; }

    /// <summary>
    /// The window's hours that no subject needed.
    /// </summary>
    public StudyHours UnusedHours { get; }

    /// <summary>
    /// The hours of study placed on <paramref name="date"/>, or zero for a date with none.
    /// </summary>
    public StudyHours HoursOn(DateOnly date) => Sum(Sessions.Where(s => s.Date == date));

    /// <summary>
    /// The hours given to <paramref name="subjectId"/>, or zero for a subject not in the schedule.
    /// </summary>
    public StudyHours HoursFor(SubjectId subjectId) => Sum(Sessions.Where(s => s.SubjectId == subjectId));

    private static StudyHours Sum(IEnumerable<StudySession> sessions) =>
        sessions.Aggregate(StudyHours.Zero, (total, s) => total + s.Hours);
}
