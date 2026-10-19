namespace Kotsu.Scheduling.Domain;

/// <summary>
/// The topics for one subject at one stage with one exam board.
/// </summary>
public sealed class Syllabus
{
    private Syllabus(StageId stageId, SubjectId subjectId, BoardId boardId, IReadOnlyList<Topic> topics)
    {
        StageId = stageId;
        SubjectId = subjectId;
        BoardId = boardId;
        Topics = topics;
    }

    public StageId StageId { get; }

    public SubjectId SubjectId { get; }

    public BoardId BoardId { get; }

    public IReadOnlyList<Topic> Topics { get; }

    public static Result<Syllabus> Create(StageId stageId, SubjectId subjectId, BoardId boardId, IEnumerable<Topic> topics)
    {
        var list = topics.ToList().AsReadOnly();

        var errors = new List<ValidationError>();
        if (list.Count == 0)
        {
            errors.Add(new("syllabus.no-topics", "A syllabus needs at least one topic."));
        }

        if (Collections.HasDuplicates(list, t => t.Id))
        {
            errors.Add(new("syllabus.duplicate-topic", "A syllabus can't list the same topic twice."));
        }

        // Progress is tracked by sub-topic id, so a sub-topic id must be unique across the whole syllabus.
        if (Collections.HasDuplicates(list.SelectMany(t => t.SubTopics.Select(s => s.Id).Distinct()), id => id))
        {
            errors.Add(new("syllabus.duplicate-sub-topic", "A sub-topic can't appear in two topics of the same syllabus."));
        }

        return Result.From(errors, () => new Syllabus(stageId, subjectId, boardId, list));
    }
}
