namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Rules for display names: not blank, no surrounding spaces, at most 100 characters.
/// </summary>
internal static class Name
{
    public const int MaxLength = 100;

    public static List<ValidationError> Validate(string? name)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new("name.blank", "A name can't be blank."));
            return errors;
        }

        if (name.Trim() != name)
        {
            errors.Add(new("name.untrimmed", "A name can't start or end with spaces."));
        }

        if (name.Length > MaxLength)
        {
            errors.Add(new("name.too-long", $"A name can be at most {MaxLength} characters."));
        }

        return errors;
    }
}
