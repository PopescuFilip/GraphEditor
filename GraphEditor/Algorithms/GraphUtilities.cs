namespace GraphEditor.Algorithms;

public static class GraphUtilities
{
    public static readonly EqualityComparer<(int, int)> OrderInsensitiveTupleComparer =
        EqualityComparer<(int, int)>.Create(OrderInsensitiveTupleEquality, a => HashCode.Combine(a.Item1, a.Item2) + HashCode.Combine(a.Item2, a.Item1));

    public static bool OrderInsensitiveTupleEquality((int, int) first, (int, int) second) =>
        first.Item1 == second.Item1 && first.Item2 == second.Item2 ||
        first.Item1 == second.Item2 && first.Item2 == second.Item1;
}

public static class GraphUtilities<T>
{
    public static readonly EqualityComparer<KeyValuePair<(int, int), T>> EdgeComparer =
        EqualityComparer<KeyValuePair<(int, int), T>>.Create(OrderInsensitiveEdgeEquality);

    public static bool OrderInsensitiveEdgeEquality(KeyValuePair<(int, int), T> first, KeyValuePair<(int, int), T> second) =>
        GraphUtilities.OrderInsensitiveTupleEquality(first.Key, second.Key);
}