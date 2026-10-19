using System.Text.RegularExpressions;

namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Rules shared by every typed id: 1–64 characters of lowercase a–z, 0–9 and single hyphens,
/// not starting or ending with a hyphen.
/// </summary>
internal static partial class IdCode
{
    public const int MaxLength = 64;

    public static List<ValidationError> Validate(string? code)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(code))
        {
            errors.Add(new("id.empty", "An id needs a code."));
            return errors;
        }

        if (!Format().IsMatch(code))
        {
            errors.Add(new("id.invalid-format", $"'{code}' must use lowercase letters, digits and single hyphens."));
        }

        if (code.Length > MaxLength)
        {
            errors.Add(new("id.too-long", $"An id code can be at most {MaxLength} characters."));
        }

        return errors;
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex Format();
}
