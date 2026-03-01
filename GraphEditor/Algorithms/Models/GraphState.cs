using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public static class GraphState
{
    public static GraphState<T> CreateRange<T>(IEnumerable<T> edges) where T : SimpleEdge =>
        GraphState<T>.Empty.AddRange(edges);
}

public record GraphState<T>(
    ImmutableDictionary<int, ImmutableArray<int>> AdjacencyList,
    ImmutableDictionary<(int, int), T> Edges)
    where T : SimpleEdge
{
    public static readonly GraphState<T> Empty = new(
        ImmutableDictionary.Create<int, ImmutableArray<int>>(),
        ImmutableDictionary.Create<(int, int), T>());

    public T this[int startNode, int endNode] => Edges[(startNode, endNode)];

    public GraphState<T> AddRange(IEnumerable<T> edges)
    {
        var collectedEdges = edges.ToImmutableArray();
        var newEdges = Edges.AddRange(collectedEdges.Select(e => new KeyValuePair<(int, int), T>((e.StartNode, e.EndNode), e)));

        var newAdjacencyList = collectedEdges
            .GroupBy(e => e.StartNode)
            .Select(g => new { StartNode = g.Key, EndNodes = g.Select(x => x.EndNode) })
            .Aggregate(AdjacencyList, (adjacencyList, edgeGrouping) =>
                adjacencyList.TryGetValue(edgeGrouping.StartNode, out var adjacentNodes)
                ? adjacencyList.SetItem(edgeGrouping.StartNode, adjacentNodes.AddRange(edgeGrouping.EndNodes))
                : adjacencyList.Add(edgeGrouping.StartNode, [.. edgeGrouping.EndNodes]));

        return new GraphState<T>(newAdjacencyList, newEdges);
    }

    public GraphState<T> AddEdge(T edge)
    {
        var newAdjacencyList = AdjacencyList.TryGetValue(edge.StartNode, out var adjacentNodes)
            ? AdjacencyList.SetItem(edge.StartNode, adjacentNodes.Add(edge.EndNode))
            : AdjacencyList.Add(edge.StartNode, [edge.EndNode]);
        var newEdges = Edges.Add((edge.StartNode, edge.EndNode), edge);

        return new GraphState<T>(newAdjacencyList, newEdges);
    }

    public IEnumerable<T> GetEdges() => Edges.Values;
}