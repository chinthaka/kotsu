namespace Kotsu.Scheduling.Domain;

/// <summary>
/// Either a value or one or more validation errors. Returned by every domain type's Create factory.
/// </summary>
public sealed class Result<T>
{
    private readonly T? _value;

    internal Result(T? value, IReadOnlyList<ValidationError> errors)
    {
        _value = value;
        Errors = errors;
    }

    public bool IsSuccess => Errors.Count == 0;

    public IReadOnlyList<ValidationError> Errors { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("A failed result has no value.");
}

public static class Result
{
    public static Result<T> Success<T>(T value) => new(value, []);

    public static Result<T> Failure<T>(IEnumerable<ValidationError> errors)
    {
        var list = errors.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("A failure needs at least one error.", nameof(errors));
        }

        return new(default, list.AsReadOnly());
    }

    /// <summary>
    /// Success when <paramref name="errors"/> is empty, otherwise a failure holding them.
    /// </summary>
    internal static Result<T> From<T>(IReadOnlyCollection<ValidationError> errors, Func<T> create) =>
        errors.Count == 0 ? Success(create()) : Failure<T>(errors);
}
