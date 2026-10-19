namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The smallest unit of a syllabus a student selects and the engine schedules.
/// </summary>
public sealed class SubTopic
{
    private static readonly StudyHours _minimumHours = StudyHours.Create(0.5m).Value;

    private SubTopic(SubTopicId id, string name, StudyHours estimatedHours)
    {
        Id = id;
        Name = name;
        EstimatedHours = estimatedHours;
    }

    public SubTopicId Id { get; }

    public string Name { get; }

    public StudyHours EstimatedHours { get; }

    public static Result<SubTopic> Create(SubTopicId id, string? name, StudyHours estimatedHours)
    {
        var errors = Domain.Name.Validate(name);
        if (estimatedHours < _minimumHours)
        {
            errors.Add(new("sub-topic.too-few-hours", "A sub-topic needs at least half an hour of study."));
        }

        return Result.From(errors, () => new SubTopic(id, name!, estimatedHours));
    }
}
