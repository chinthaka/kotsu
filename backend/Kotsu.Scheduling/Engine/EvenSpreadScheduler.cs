using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Engine;

/// <summary>
/// Places every selected sub-topic into a study calendar, sharing the hours evenly across subjects.
/// Works in three steps, each its own method so later phases can replace one: share the window's
/// half-hours across subjects, split each subject's half-hours across its sub-topics, then place
/// them on dates.
/// </summary>
public static class EvenSpreadScheduler
{
    public static Result<Schedule> Build(StudyCalendar calendar, IReadOnlyList<SubjectSelection> selections)
    {
        var errors = Validate(calendar, selections);
        if (errors.Count > 0)
        {
            return Result.Failure<Schedule>(errors);
        }

        var total = HalfHours.Of(calendar.TotalHours);
        var shares = Share(total, selections);
        var portions = Split(selections, shares);
        var sessions = Place(calendar, selections, portions);

        var placed = sessions.Sum(s => HalfHours.Of(s.Hours));
        return Result.Success(new Schedule(calendar.Window, sessions.AsReadOnly(), HalfHours.ToHours(total - placed)));
    }

    private static List<ValidationError> Validate(StudyCalendar calendar, IReadOnlyList<SubjectSelection> selections)
    {
        var errors = new List<ValidationError>();
        if (selections.Count == 0)
        {
            errors.Add(new("schedule.no-subjects", "A schedule needs at least one subject."));
        }

        if (Collections.HasDuplicates(selections, s => s.SubjectId))
        {
            errors.Add(new("schedule.duplicate-subject", "A schedule can't include the same subject twice."));
        }

        if (selections.Select(s => s.Syllabus.StageId).Distinct().Skip(1).Any())
        {
            errors.Add(new("schedule.mixed-stage", "Every subject in a schedule must be from the same stage."));
        }

        var subTopicCount = selections.Sum(s => s.SubTopics.Count);
        if (HalfHours.Of(calendar.TotalHours) < subTopicCount)
        {
            errors.Add(new("schedule.not-enough-hours", "There isn't enough study time for every selected sub-topic."));
        }

        return errors;
    }

    /// <summary>
    /// Shares the window's half-hours evenly across subjects. A subject never gets fewer half-hours than
    /// it has sub-topics, and never more than it needs. Half-hours a subject doesn't need go to the subjects
    /// that still need more, evenly. Spare half-hours go to subjects in the order they were given.
    /// </summary>
    private static int[] Share(int total, IReadOnlyList<SubjectSelection> selections)
    {
        var lower = selections.Select(s => s.SubTopics.Count).ToArray();
        var upper = selections.Select(s => s.SubTopics.Sum(t => HalfHours.Of(t.EstimatedHours))).ToArray();

        var shares = (int[])lower.Clone();
        var remaining = Math.Min(total, upper.Sum()) - lower.Sum();
        while (remaining > 0)
        {
            var open = Enumerable.Range(0, shares.Length).Where(i => shares[i] < upper[i]).ToList();
            var lowest = open.Min(i => shares[i]);
            foreach (var i in open.Where(i => shares[i] == lowest))
            {
                if (remaining == 0)
                {
                    break;
                }

                shares[i]++;
                remaining--;
            }
        }

        return shares;
    }

    /// <summary>
    /// Splits each subject's half-hours evenly across its selected sub-topics, in syllabus order.
    /// Spare half-hours go to the earliest sub-topics.
    /// </summary>
    private static Portion[][] Split(IReadOnlyList<SubjectSelection> selections, int[] shares) =>
        selections.Select((selection, i) =>
        {
            var count = selection.SubTopics.Count;
            var each = shares[i] / count;
            var spare = shares[i] % count;
            return selection.SubTopics
                .Select((subTopic, j) => new Portion(subTopic.Id, each + (j < spare ? 1 : 0)))
                .ToArray();
        }).ToArray();

    /// <summary>
    /// Fills dates in order. Each date's half-hours are handed out one at a time, in turn, to the subjects
    /// that still have time to place, starting with a different subject each date. Inside a subject,
    /// sub-topics are placed in syllabus order, carrying on into the next date when one doesn't fit.
    /// </summary>
    private static List<StudySession> Place(StudyCalendar calendar, IReadOnlyList<SubjectSelection> selections, Portion[][] portions)
    {
        var remaining = portions.Select(subject => subject.Select(p => p.HalfHours).ToArray()).ToArray();
        var sessions = new List<StudySession>();

        for (var dateIndex = 0; dateIndex < calendar.Days.Count; dateIndex++)
        {
            var day = calendar.Days[dateIndex];
            var turns = TurnOrder(remaining, startingWith: dateIndex % selections.Count);
            if (turns.Count == 0)
            {
                break;
            }

            var today = remaining.Select(subject => new int[subject.Length]).ToArray();
            var slots = HalfHours.Of(day.Hours);
            while (slots > 0 && turns.Count > 0)
            {
                foreach (var i in turns.ToList())
                {
                    if (slots == 0)
                    {
                        break;
                    }

                    var j = Array.FindIndex(remaining[i], left => left > 0);
                    remaining[i][j]--;
                    today[i][j]++;
                    slots--;
                    if (remaining[i].All(left => left == 0))
                    {
                        turns.Remove(i);
                    }
                }
            }

            for (var i = 0; i < selections.Count; i++)
            {
                for (var j = 0; j < portions[i].Length; j++)
                {
                    if (today[i][j] > 0)
                    {
                        sessions.Add(new StudySession(day.Date, selections[i].SubjectId, portions[i][j].SubTopicId, HalfHours.ToHours(today[i][j])));
                    }
                }
            }
        }

        return sessions;
    }

    /// <summary>
    /// The subjects that still have time to place, starting from <paramref name="startingWith"/> and
    /// wrapping round, so that a different subject goes first each date.
    /// </summary>
    private static List<int> TurnOrder(int[][] remaining, int startingWith) =>
        Enumerable.Range(0, remaining.Length)
            .Select(offset => (startingWith + offset) % remaining.Length)
            .Where(i => remaining[i].Any(left => left > 0))
            .ToList();

    private readonly record struct Portion(SubTopicId SubTopicId, int HalfHours);
}
