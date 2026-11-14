namespace Kotsu.Scheduling.Domain;

/// <summary>
/// One subject's syllabus and the sub-topics the student selected from it: the whole
/// syllabus, or specific sub-topics. The selected sub-topics are kept in syllabus order.
/// </summary>
public sealed class SubjectSelection
{
    private SubjectSelection(Syllabus syllabus, IReadOnlyList<SubTopic> subTopics)
    {
        Syllabus = syllabus;
        SubTopics = subTopics;
    }

    public Syllabus Syllabus { get; }

    public SubjectId SubjectId => Syllabus.SubjectId;

    /// <summary>
    /// The selected sub-topics, in syllabus order.
    /// </summary>
    public IReadOnlyList<SubTopic> SubTopics { get; }

    /// <summary>
    /// Selects the whole syllabus. Always succeeds: the syllabus is already valid.
    /// </summary>
    public static Result<SubjectSelection> Create(Syllabus syllabus) =>
        Result.Success(new SubjectSelection(syllabus, AllSubTopics(syllabus).ToList().AsReadOnly()));

    /// <summary>
    /// Selects specific sub-topics. Repeated ids are merged.
    /// </summary>
    public static Result<SubjectSelection> Create(Syllabus syllabus, IEnumerable<SubTopicId> subTopicIds)
    {
        var wanted = subTopicIds.ToHashSet();

        var errors = new List<ValidationError>();
        if (wanted.Count == 0)
        {
            errors.Add(new("subject-selection.no-sub-topics", "Select at least one sub-topic."));
        }

        var known = AllSubTopics(syllabus).Select(s => s.Id).ToHashSet();
        if (wanted.Any(id => !known.Contains(id)))
        {
            errors.Add(new("subject-selection.unknown-sub-topic", "A selected sub-topic isn't in this syllabus."));
        }

        return Result.From(errors, () =>
            new SubjectSelection(syllabus, AllSubTopics(syllabus).Where(s => wanted.Contains(s.Id)).ToList().AsReadOnly()));
    }

    private static IEnumerable<SubTopic> AllSubTopics(Syllabus syllabus) => syllabus.Topics.SelectMany(t => t.SubTopics);
}
