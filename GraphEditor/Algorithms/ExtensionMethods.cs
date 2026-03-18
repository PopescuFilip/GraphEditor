namespace GraphEditor.Algorithms;

public static class ExtensionMethods
{
    public static (T, T) Swap<T>(this (T, T) value) => (value.Item2, value.Item1);

    public static void Shuffle<T>(this List<T> list)
    {
        var random = new Random();

        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static IEnumerable<T> SelectStartingWith<T>(this IReadOnlyDictionary<T, T> parents, T start)
    {
        var current = start;
        while (parents.TryGetValue(current, out var next))
        {
            yield return current;
            current = next;
        }

        yield return current;
    }

    public static IEnumerable<(T, T)> SelectPairs<T>(this IEnumerable<T> values)
    {
        var previousValueInitilized = false;
        T? previousValue = default;

        foreach (var value in values)
        {
            if (!previousValueInitilized)
            {
                previousValue = value;
                previousValueInitilized = true;
                continue;
            }

            yield return (previousValue!, value);
            previousValue = value;
        }
    }

    public static IEnumerable<T> ToEnumerable<T>(this (T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
    }
}