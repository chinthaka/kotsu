namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A broken validation rule. <see cref="Code"/> is stable and safe to match on;
/// <see cref="Message"/> is for people and may change.
/// </summary>
public sealed record ValidationError(string Code, string Message);
