namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A syllabus topic, made of one or more sub-topics.
/// </summary>
public sealed class Topic
{
    private Topic(TopicId id, string name, IReadOnlyList<SubTopic> subTopics)
    {
        Id = id;
        Name = name;
        SubTopics = subTopics;
        EstimatedHours = subTopics.Aggregate(StudyHours.Zero, (total, s) => total + s.EstimatedHours);
    }

    public TopicId Id { get; }

    public string Name { get; }

    public IReadOnlyList<SubTopic> SubTopics { get; }

    /// <summary>
    /// The sum of the sub-topics' estimated hours.
    /// </summary>
    public StudyHours EstimatedHours { get; }

    public static Result<Topic> Create(TopicId id, string? name, IEnumerable<SubTopic> subTopics)
    {
        var list = subTopics.ToList().AsReadOnly();

        var errors = Domain.Name.Validate(name);
        if (list.Count == 0)
        {
            errors.Add(new("topic.no-sub-topics", "A topic needs at least one sub-topic."));
        }

        if (Collections.HasDuplicates(list, s => s.Id))
        {
            errors.Add(new("topic.duplicate-sub-topic", "A topic can't list the same sub-topic twice."));
        }

        return Result.From(errors, () => new Topic(id, name!, list));
    }
}
