namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Identifies a country by a readable code.
/// </summary>
public readonly record struct CountryId
{
    private CountryId(string code) => Code = code;

    public string Code { get; }

    public static Result<CountryId> Create(string? code) => Result.From(IdCode.Validate(code), () => new CountryId(code!));

    public override string ToString() => Code;
}
