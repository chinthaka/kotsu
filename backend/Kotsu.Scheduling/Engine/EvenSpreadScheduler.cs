using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Engine;

/// <summary>
/// Places every selected sub-topic into a study calendar, sharing the hours evenly across subjects.
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

        return Result.Success(new Schedule(calendar.Window, [], calendar.TotalHours));
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
}
