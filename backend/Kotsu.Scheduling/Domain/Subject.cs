namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A subject a student can study, such as Mathematics.
/// </summary>
public sealed class Subject
{
    private Subject(SubjectId id, string name)
    {
        Id = id;
        Name = name;
    }

    public SubjectId Id { get; }

    public string Name { get; }

    public static Result<Subject> Create(SubjectId id, string? name) =>
        Result.From(Domain.Name.Validate(name), () => new Subject(id, name!));
}
