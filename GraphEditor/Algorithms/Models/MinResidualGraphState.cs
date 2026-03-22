using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public record MinResidualGraphState(
    ImmutableDictionary<int, ImmutableArray<int>> AdjacencyList,
    ImmutableDictionary<(int, int), ResidualEdge> Edges,
    int MinResidualValue)
    : GraphState<ResidualEdge>(AdjacencyList, Edges);

public static class MinResidualGraphStateExtensions
{
    public static MinResidualGraphState ToMinResidual(this GraphState<ResidualEdge> graphState, int minResidualValue)
    {
        var eligibleEdges = graphState.Edges.Values.Where(e => e.ResidualValue >= minResidualValue);
        var newGraphState = GraphState.CreateRange(eligibleEdges);

        return new MinResidualGraphState(newGraphState.AdjacencyList, newGraphState.Edges, minResidualValue);
    }
}