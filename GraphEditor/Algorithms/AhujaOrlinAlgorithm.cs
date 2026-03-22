using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms;

public class AhujaOrlinAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var maxFlow = initialFlow;
        var graphState = GraphState.CreateRange(graph.Edges);
        var residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var maxResidual = residualGraph.Edges.Values.Select(x => x.ResidualValue).Max();
        var maxPowerOfTwo = MathUtilities.GetMaxPowerOfTwoLessThan(maxResidual);

        while (maxPowerOfTwo != 0)
        {
            var minResidualGraph = residualGraph.ToMinResidual(maxPowerOfTwo);
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. minResidualGraph.Edges.Values]));

            while (TryFindWayToEndNode(minResidualGraph, startNode, endNode, out var wayToEndNode))
            {
                var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
                maxFlow += maxWayFlow;
                graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
                residualGraph = graphState.ToResidual();
                minResidualGraph = residualGraph.ToMinResidual(maxPowerOfTwo);
                onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. minResidualGraph.Edges.Values]));
            }

            maxPowerOfTwo /= 2;
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
}