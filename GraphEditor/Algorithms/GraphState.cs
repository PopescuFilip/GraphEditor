using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms;

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

    public GraphState<T> AddRange(IEnumerable<T> edges)
    {
        var newEdges = Edges.AddRange(edges.Select(e => new KeyValuePair<(int, int), T>((e.StartNode, e.EndNode), e)));

        var newAdjacencyList = edges
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

public static class FlowGraphStateExtensions
{
    public static GraphState<ResidualEdge> ToResidual(this GraphState<FlowEdge> flowGraphState)
    {
        var processedEdges = new HashSet<(int, int)>();
        var residualGraphState = GraphState<ResidualEdge>.Empty;

        foreach (var edge in flowGraphState.Edges)
        {
            if (processedEdges.Contains(edge.Key))
                continue;

            var (start, end, _, _) = edge.Value;

            var residualValue = flowGraphState.ResidualValueFor(edge.Key);
            if (residualValue > 0)
            {
                residualGraphState = residualGraphState.AddEdge(new ResidualEdge(start, end, residualValue));
            }
            processedEdges.Add(edge.Key);

            var inverseResidualValue = flowGraphState.ResidualValueFor(edge.Key.Swap());
            if (inverseResidualValue > 0)
            {
                residualGraphState = residualGraphState.AddEdge(new ResidualEdge(end, start, inverseResidualValue));
            }
            processedEdges.Add(edge.Key.Swap());
        }

        return residualGraphState;
    }

    private static int ResidualValueFor(this GraphState<FlowEdge> graphState, (int, int) startEnd)
    {
        var capacity = graphState.Edges.TryGetValue(startEnd, out var edge)
            ? edge.Capacity
            : 0;

        var flow = edge?.Flow ?? 0;
        var inverseFlow = graphState.Edges.TryGetValue(startEnd.Swap(), out var inverseEdge)
            ? inverseEdge.Flow
            : 0;

        return capacity - flow + inverseFlow;
    }
}