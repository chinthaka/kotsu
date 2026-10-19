namespace Kotsu.Scheduling.Domain;

internal static class Collections
{
    public static bool HasDuplicates<T, TKey>(IEnumerable<T> items, Func<T, TKey> key) =>
        items.GroupBy(key).Any(group => group.Skip(1).Any());
}
