using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms;

public class GenericAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow)
    {
        var maxFlow = initialFlow;
        var graphState =  GraphState.CreateRange(graph.Edges);
        var residualGraph = graphState.ToResidual();
        while(TryFindWayToEndNode(residualGraph, startNode, endNode, out var wayToEndNode))
        {
            var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
            maxFlow += maxWayFlow;
            graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
            residualGraph = graphState.ToResidual();
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, int startNode, int endNode, [NotNullWhen(true)]out Way? wayToEndNode)
    {
        wayToEndNode = null;

        var seenNodes = new List<int>() { startNode };
        var nodesToVisit = new List<int>() { startNode };
        var visitedNodes = new List<int>();
        var parents = new Dictionary<int, int>();

        while(nodesToVisit.Count != 0)
        {
            nodesToVisit.Shuffle();
            var currentNode = nodesToVisit[0];

            if (!graphState.AdjacencyList.TryGetValue(currentNode, out var adjacentNodes))
            {
                visitedNodes.Add(currentNode);
                nodesToVisit.RemoveAt(0);
                continue;
            }

            var undiscoveredNodes = adjacentNodes
                .Where(x => !seenNodes.Contains(x))
                .ToList();
            if (undiscoveredNodes.Count != 0)
            {
                undiscoveredNodes.Shuffle();
                var newNode = undiscoveredNodes[0];
                parents[newNode] = currentNode;
                seenNodes.Add(newNode);
                nodesToVisit.Add(newNode);
            }
            else
            {
                visitedNodes.Add(currentNode);
                nodesToVisit.RemoveAt(0);
            }
        }

        if (!parents.ContainsKey(endNode))
            return false;

        var edges = parents.SelectStartingWith(endNode).Reverse().SelectPairs();
        wayToEndNode = new Way([.. edges]);
        return true;
    }
}