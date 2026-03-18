using GraphEditor.Models;
using System.Collections.Immutable;
using KvpFlowEdge = System.Collections.Generic.KeyValuePair<(int, int), GraphEditor.Models.FlowEdge>;

namespace GraphEditor.Algorithms.Models;

public static class FlowGraphExtensions
{
    public static GraphState<FlowEdge> FixFlow(this GraphState<FlowEdge> flowGraphState)
    {
        var overflowedEdges = flowGraphState.Edges
            .Where(x => x.Value.Flow > x.Value.Capacity)
            .ToImmutableDictionary(x => x.Key, x => x.Value);

        if (overflowedEdges.Count == 0)
            return flowGraphState;

        var newEdges = flowGraphState.Edges
            .GroupBy(x => overflowedEdges.ContainsKey(x.Key) || overflowedEdges.ContainsKey(x.Key.Swap()))
            .Select(x => new { ShouldChange = x.Key, Edges = x.Select(y => y) })
            .Select(x => x.ShouldChange
                ? x.Edges.SelectFixedEdges()
                : x.Edges)
            .SelectMany(x => x)
            .ToImmutableDictionary();

        return flowGraphState with { Edges = newEdges };
    }

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

    private static IEnumerable<KvpFlowEdge> SelectFixedEdges(this IEnumerable<KvpFlowEdge> flowEdges) =>
        flowEdges
            .GroupBy(x => x.Key, GraphUtilities.OrderInsensitiveTupleComparer)
            .SelectMany(x => x.SelectPairs().First().ToFixedPair().ToEnumerable());

    private static (KvpFlowEdge, KvpFlowEdge) ToFixedPair(this (KvpFlowEdge, KvpFlowEdge) pair) => pair switch
    {
        (var a, var b) when a.Value.Capacity < a.Value.Flow => (
        KeyValuePair.Create(a.Key, a.Value with { Flow = a.Value.Flow - b.Value.Flow }),
        KeyValuePair.Create(b.Key, b.Value with { Flow = 0 })),
        (var _, var b) when b.Value.Capacity < b.Value.Flow => pair.Swap().ToFixedPair(),
        _ => throw new InvalidOperationException()
    };
}