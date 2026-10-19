namespace Kotsu.Scheduling.Domain;

/// <summary>
/// An exam board that sets and marks exams, such as AQA or SQA.
/// </summary>
public sealed class ExamBoard
{
    private ExamBoard(BoardId id, string name)
    {
        Id = id;
        Name = name;
    }

    public BoardId Id { get; }

    public string Name { get; }

    public static Result<ExamBoard> Create(BoardId id, string? name) =>
        Result.From(Domain.Name.Validate(name), () => new ExamBoard(id, name!));
}
