namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a exam board by a readable code.
/// </summary>
public readonly record struct BoardId
{
    private BoardId(string code) => Code = code;

    public string Code { get; }

    public static Result<BoardId> Create(string? code) => Result.From(IdCode.Validate(code), () => new BoardId(code!));

    public override string ToString() => Code;
}
