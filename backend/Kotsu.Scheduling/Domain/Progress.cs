namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Which sub-topics the student has marked complete.
/// </summary>
public sealed class Progress
{
    private Progress(IReadOnlySet<SubTopicId> completed) => Completed = completed;

    public IReadOnlySet<SubTopicId> Completed { get; }

    public bool IsComplete(SubTopicId subTopicId) => Completed.Contains(subTopicId);

    /// <summary>
    /// Duplicates are merged: marking a sub-topic complete twice is harmless.
    /// </summary>
    public static Result<Progress> Create(IEnumerable<SubTopicId> completed) =>
        Result.Success(new Progress(completed.ToHashSet().AsReadOnly()));
}
