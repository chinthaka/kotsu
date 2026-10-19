namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a subject by a readable code.
/// </summary>
public readonly record struct SubjectId
{
    private SubjectId(string code) => Code = code;

    public string Code { get; }

    public static Result<SubjectId> Create(string? code) => Result.From(IdCode.Validate(code), () => new SubjectId(code!));

    public override string ToString() => Code;
}
