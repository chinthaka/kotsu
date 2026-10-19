namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a syllabus topic by a readable code.
/// </summary>
public readonly record struct TopicId
{
    private TopicId(string code) => Code = code;

    public string Code { get; }

    public static Result<TopicId> Create(string? code) => Result.From(IdCode.Validate(code), () => new TopicId(code!));

    public override string ToString() => Code;
}
