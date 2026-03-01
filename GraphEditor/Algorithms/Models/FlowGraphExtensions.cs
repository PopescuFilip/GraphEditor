using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public static class FlowGraphExtensions
{
    public static GraphState<FlowEdge> AddFlow(this GraphState<FlowEdge> flowGraphState, Way way, int flow)
    {
        var newEdges = flowGraphState.Edges
            .GroupBy(x => way.Edges.Contains(x.Key))
            .Select(x => new { ShouldChange = x.Key, Edges = x.Select(y => y.Value) })
            .Select(x => x.ShouldChange
                ? x.Edges.Select(x => x with { Flow = x.Flow + flow })
                : x.Edges)
            .SelectMany(x => x)
            .ToImmutableDictionary(x => (x.StartNode, x.EndNode), x => x);

        return flowGraphState with { Edges = newEdges };
    }

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