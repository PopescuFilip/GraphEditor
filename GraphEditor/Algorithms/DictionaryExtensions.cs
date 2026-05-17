using System.Collections.Immutable;

namespace GraphEditor.Algorithms;

public static class DictionaryExtensions
{
    public static TValue TryGetValueOrDefault<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        where TKey : notnull
        =>
        dict.TryGetValue(key, out var value) ? value : defaultValue;
}