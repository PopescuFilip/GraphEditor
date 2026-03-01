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

        }

        return (maxFlux, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, int startNode, int endNode, out ImmutableArray<int> wayToEndNode)
    {
        wayToEndNode = [];

        var nodesToVisit = new List<int>() { startNode };
        var visitedNodes = new List<int>();
        var visitedBy = new Dictionary<int, int>();

        while(nodesToVisit.Count != 0)
        {
            var nodeToVisit = nodesToVisit[0];
            visitedNodes.Add(nodeToVisit);
            nodesToVisit.RemoveAt(0);

            if (!graphState.AdjacencyList.TryGetValue(nodeToVisit, out var adjacentNodes))
                continue;

            var notVisitedNodes = adjacentNodes.Where(x => !visitedNodes.Contains(x)).ToList();
            if (notVisitedNodes.Count != 0)
            {
                notVisitedNodes.Shuffle();
                nodesToVisit.AddRange(notVisitedNodes);
            }
        }

        if (!visitedBy.ContainsKey(endNode))
            return false;

        var way = new List<int>();
        var currentNode = endNode;
        while (visitedBy.TryGetValue(currentNode, out var nextNode))
        {
            way.Add(currentNode);
            currentNode = nextNode;
        }
        if (currentNode != startNode)
            return false;

        way.Add(currentNode);
        way.Reverse();
        wayToEndNode = [.. way];
        return true;
    }
}