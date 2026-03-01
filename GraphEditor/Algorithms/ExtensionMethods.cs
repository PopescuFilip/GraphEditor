namespace GraphEditor.Algorithms;

public static class ExtensionMethods
{
    public static (int, int) Swap(this (int, int) value) => (value.Item2, value.Item1);

    public static void Shuffle<T>(this List<T> list)
    {
        var random = new Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static IEnumerable<(T, T)> SelectPairs<T>(this IEnumerable<T> values)
    {
        bool previousValueInitilized = false;
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
}