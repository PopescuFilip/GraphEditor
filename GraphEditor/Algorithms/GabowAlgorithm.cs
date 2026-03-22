using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms;

public class GabowAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var maxFlow = initialFlow;
        var graphState = GraphState.CreateRange(graph.Edges);

        var stack = new Stack<CapacityContainer>();
        foreach (var current in GetCapacitiesHalved(graphState))
        {
            stack.Push(current);
        }

        while (stack.TryPop(out var currentCapacity))
        {
            maxFlow *= 2;
            graphState = ApplyCapacities(graphState, currentCapacity).DoubleFlow();
            var residualGraph = graphState.ToResidual();
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

            while (TryFindWayToEndNode(residualGraph, startNode, endNode, out var wayToEndNode))
            {
                var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
                maxFlow += maxWayFlow;
                graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
                residualGraph = graphState.ToResidual();
                onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
            }
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, int startNode, int endNode, [NotNullWhen(true)] out Way? wayToEndNode)
    {
        wayToEndNode = null;

        var nodesToVisit = new List<int>() { startNode };
        var visitedNodes = new List<int>();
        var parents = new Dictionary<int, int>();

        while (nodesToVisit.Count != 0)
        {
            var currentNode = nodesToVisit[0];
            visitedNodes.Add(currentNode);
            nodesToVisit.RemoveAt(0);

            if (!graphState.AdjacencyList.TryGetValue(currentNode, out var adjacentNodes))
                continue;

            var undiscoveredNodes = adjacentNodes
                .Where(x => !visitedNodes.Contains(x) && !nodesToVisit.Contains(x))
                .ToList();
            if (undiscoveredNodes.Count != 0)
            {
                foreach (var undiscoveredNode in undiscoveredNodes)
                {
                    parents[undiscoveredNode] = currentNode;
                }
                undiscoveredNodes.Shuffle();
                nodesToVisit.AddRange(undiscoveredNodes);
            }
        }

        if (!parents.ContainsKey(endNode))
            return false;

        var edges = parents.SelectStartingWith(endNode).Reverse().SelectPairs();
        wayToEndNode = new Way([.. edges]);
        return true;
    }

    private GraphState<FlowEdge> ApplyCapacities(GraphState<FlowEdge> graphState, CapacityContainer capacityContainer)
    {
        var newEdges = graphState.Edges
            .Select(x => KeyValuePair.Create(x.Key, x.Value with { Capacity = capacityContainer.Capacities[x.Key] }))
            .ToImmutableDictionary();

        return graphState with { Edges = newEdges };
    }

    private IEnumerable<CapacityContainer> GetCapacitiesHalved(GraphState<FlowEdge> graphState)
    {
        var graphCapacities = graphState.Edges.Select(x => KeyValuePair.Create(x.Key, x.Value.Capacity)).ToImmutableDictionary();
        var currentContainer = new CapacityContainer(graphCapacities);
        yield return currentContainer;

        while (currentContainer.Capacities.Values.Any(x => x > 1))
        {
            var newCapacities = currentContainer.Capacities.Select(x => KeyValuePair.Create(x.Key, x.Value / 2)).ToImmutableDictionary();
            currentContainer = new CapacityContainer(newCapacities);
            yield return currentContainer;
        }
    }

    private record CapacityContainer(ImmutableDictionary<(int, int), int> Capacities);
}