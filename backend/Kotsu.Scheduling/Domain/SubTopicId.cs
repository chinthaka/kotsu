namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a sub-topic by a readable code.
/// </summary>
public readonly record struct SubTopicId
{
    private SubTopicId(string code) => Code = code;

    public string Code { get; }

    public static Result<SubTopicId> Create(string? code) => Result.From(IdCode.Validate(code), () => new SubTopicId(code!));

    public override string ToString() => Code;
}
