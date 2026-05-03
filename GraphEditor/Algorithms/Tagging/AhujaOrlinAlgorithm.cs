using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms.Tagging;

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

        var tags = residualGraph.GetDistanceTags(endNode);
        while (TryFindWayToEndNode(residualGraph, startNode, endNode, tags, graph.Nodes.Length, out var wayToEndNode))
        {
            var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
            maxFlow += maxWayFlow;
            graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
            residualGraph = graphState.ToResidual();
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, int startNode, int endNode,
        Dictionary<int, int> tags,
        int nodesCount,
        [NotNullWhen(true)] out Way? wayToEndNode)
    {
        wayToEndNode = null;

        var parents = new Dictionary<int, int>();
        var currentNode = startNode;

        while (tags[startNode] < nodesCount)
        {
            var admissibleNode = graphState.GetAdmissibleNodes(currentNode, tags).FirstOrDefault();
            if (admissibleNode is default(int))
            {
                var minDistance = graphState.AdjacencyList[currentNode].Select(node => tags[node]).Min();
                tags[currentNode] = minDistance + 1;

                if (currentNode != startNode)
                    currentNode = parents[currentNode];
                continue;
            }

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