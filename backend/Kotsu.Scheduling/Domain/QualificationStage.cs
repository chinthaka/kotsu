namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A stage of study within a country, such as KS4 or SQA National 5. Stages are data, not code.
/// </summary>
public sealed class QualificationStage
{
    private QualificationStage(StageId id, string name)
    {
        Id = id;
        Name = name;
    }

    public StageId Id { get; }

    public string Name { get; }

    public static Result<QualificationStage> Create(StageId id, string? name) =>
        Result.From(Domain.Name.Validate(name), () => new QualificationStage(id, name!));
}
