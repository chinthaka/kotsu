namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a qualification stage by a readable code.
/// </summary>
public readonly record struct StageId
{
    private StageId(string code) => Code = code;

    public string Code { get; }

    public static Result<StageId> Create(string? code) => Result.From(IdCode.Validate(code), () => new StageId(code!));

    public override string ToString() => Code;
}
