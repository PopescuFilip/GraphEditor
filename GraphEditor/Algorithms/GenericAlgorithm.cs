using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms;

public class GenericAlgorithm
{
    public (int MaxFlux, Graph ResultingGraph) Run(Graph graph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0);

    public (int MaxFlux, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow)
    {
        var maxFlux = initialFlow;
        var graphState =  GraphState.CreateRange(graph.Edges);
        var residualGraph = graphState.ToResidual();
        while(TryFindWayToEndNode(residualGraph, startNode, endNode, out var wayToEndNode))
        {
            var way = wayToEndNode.SelectPairs().ToImmutableArray();
            var maxWayFlux = residualGraph.GetMinResidualValue(way);
            maxFlux += maxWayFlux;
            graphState = graphState.AddFlux(way);
            residualGraph = graphState.ToResidual();
        }

        return (maxFlux, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, int startNode, int endNode, out ImmutableArray<int> wayToEndNode)
    {
        wayToEndNode = [];

        var nodesToVisit = new List<int>() { startNode };
        var visitedNodes = new List<int>();
        var parents = new Dictionary<int, int>();

        while(nodesToVisit.Count != 0)
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

        var way = new List<int>();
        var current = endNode;
        while (parents.TryGetValue(current, out var nextNode))
        {
            way.Add(current);
            current = nextNode;
        }
        if (current != startNode)
            return false;

        way.Add(current);
        way.Reverse();
        wayToEndNode = [.. way];
        return true;
    }
}