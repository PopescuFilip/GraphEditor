using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public record ExcessGraphState(
    ImmutableDictionary<int, ImmutableArray<int>> AdjacencyList,
    ImmutableDictionary<(int, int), FlowEdge> Edges,
    ImmutableDictionary<int, int> Excess)
    : GraphState<FlowEdge>(AdjacencyList, Edges);

public static class ExcessGraphStateExtensions
{
    public static IEnumerable<int> GetActiveNodes(this ExcessGraphState graphState, int endNode) =>
        graphState.Excess.Values.Where(x => x > 0 && x != endNode);

    public static ExcessGraphState ToExcess(this GraphState<FlowEdge> graphState) =>
        new(graphState.AdjacencyList, graphState.Edges, ImmutableDictionary<int, int>.Empty);

    public static ExcessGraphState AddFlow(this ExcessGraphState graphState, (int, int) edgeKey, int flow)
    {
        var edge = graphState.Edges[edgeKey];
        return graphState.AddFlow(edge, flow);
    }

    public static ExcessGraphState AddFlow(this ExcessGraphState graphState, FlowEdge edge, int flow)
    {
        var edgeKey = (edge.StartNode, edge.EndNode);
        var newEdge = edge with { Flow = edge.Flow + flow };
        var newEdges = graphState.Edges.SetItem(edgeKey, newEdge);

        var initialStartExcess = graphState.Excess.TryGetValueOrDefault(edge.StartNode, 0);
        var newStartExcess = initialStartExcess - flow;

        var initialEndExcess = graphState.Excess.TryGetValueOrDefault(edge.EndNode, 0);
        var newEndExcess = initialEndExcess + flow;

        var newExcess = graphState.Excess.SetItems(
        [
            KeyValuePair.Create(edge.StartNode, newStartExcess),
            KeyValuePair.Create(edge.EndNode, newEndExcess),
        ]);

        return graphState with { Edges = newEdges, Excess = newExcess };
    }
}