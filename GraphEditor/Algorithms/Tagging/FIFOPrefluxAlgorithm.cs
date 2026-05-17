using GraphEditor.Algorithms.Models;
using GraphEditor.Models;

namespace GraphEditor.Algorithms.Tagging;

public class FIFOPrefluxAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var graphState = GraphState.CreateRange(graph.Edges).ToExcess();
        var residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var tags = residualGraph.GetDistanceTags(endNode);
        tags[startNode] = graph.Nodes.Length;
        graphState = SetInitialFlow(graphState, startNode);
        residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        while (graphState.GetActiveNodes(endNode).Any())
        {
            var activeNodes = graphState.GetActiveNodes(endNode).ToList();
            activeNodes.Shuffle();
            var currentNode = activeNodes.First();

            var admissibleNodes = residualGraph.GetAdmissibleNodes(currentNode, tags).ToList();

            if (admissibleNodes.Count == 0)
            {
                var minDistance = residualGraph.AdjacencyList[currentNode].Select(node => tags[node]).Min();
                tags[currentNode] = minDistance + 1;
            }
            else
            {
                admissibleNodes.Shuffle();
                var admissibleNode = admissibleNodes.First();
                var currentKey = (currentNode, admissibleNode);
                var residualEdge = residualGraph.Edges[currentKey];

                var flowToAdd = Math.Min(graphState.Excess.TryGetValueOrDefault(currentNode, 0), residualEdge.ResidualValue);
                graphState = graphState.AddFlow(currentKey, flowToAdd);

                residualGraph = graphState.ToResidual();
                onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
            }
        }

        var maxFlow = graphState.Excess[endNode];
        return (maxFlow, graph with { Edges = [.. graphState.FixFlow().GetEdges()] });
    }

    private ExcessGraphState SetInitialFlow(ExcessGraphState graphState, int startNode)
    {
        var newGraphState = graphState with { };

        foreach (var adjacentNode in graphState.AdjacencyList[startNode])
        {
            var edgeKey = (startNode, adjacentNode);
            var edge = graphState.Edges[edgeKey];
            newGraphState = newGraphState.AddFlow(edge, edge.Capacity);
        }

        return newGraphState;
    }
}
