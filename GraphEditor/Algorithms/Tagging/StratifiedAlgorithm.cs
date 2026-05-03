using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms.Tagging;

public class StratifiedAlgorithm : IAlgorithm
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

        var wayFinder = new WayFinder(startNode, endNode, residualGraph, graph.Nodes.Length);

        while (wayFinder.TryFindWayToEndNode(residualGraph, out var wayToEndNode))
        {
            var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
            maxFlow += maxWayFlow;
            graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
            residualGraph = graphState.ToResidual();
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private class WayFinder(int startNode, int endNode, GraphState<ResidualEdge> initialGraphState, int nodesCount) : IWayFinder
    {
        private Dictionary<int, int> _tags = initialGraphState.GetDistanceTags(endNode);

        public bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, [NotNullWhen(true)] out Way? wayToEndNode)
        {
            wayToEndNode = null;

            var parents = new Dictionary<int, int>();
            var currentNode = startNode;

            while (_tags[startNode] < nodesCount)
            {
                var admissibleNodes = graphState.GetAdmissibleNodes(currentNode, _tags).ToList();
                if (admissibleNodes.Count == 0)
                {
                    var minDistance = graphState.AdjacencyList[currentNode].Select(node => _tags[node]).Min();
                    _tags[currentNode] = minDistance + 1;

                    if (currentNode != startNode)
                        currentNode = parents[currentNode];
                    continue;
                }

                admissibleNodes.Shuffle();
                var admissibleNode = admissibleNodes.First();
                parents[admissibleNode] = currentNode;
                currentNode = admissibleNode;

                if (currentNode == endNode)
                {
                    wayToEndNode = parents.ToWay(endNode);
                    return true;
                }
            }

            return false;
        }
    }
}