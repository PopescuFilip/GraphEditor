using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public record Way(ImmutableArray<(int StartNode, int EndNode)> Edges);

public static class WayExtensions
{
    public static Way Reverse(this Way way) =>
        way with { Edges = [.. way.Edges.Select(x => x.Swap())] };
}