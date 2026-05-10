using GraphEditor.Algorithms.Models;
using GraphEditor.Models;

namespace GraphEditor.Algorithms.Tagging;

public class GenericPrefluxAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var maxFlow = initialFlow;
        var graphState = GraphState.CreateRange(graph.Edges).ToExcess();
        var residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var tags = residualGraph.GetDistanceTags(endNode);
        tags[startNode] = graph.Nodes.Length;
        graphState = SetInitialFlow(graphState, startNode);

        while (true)
        {
            var wayToEndNode = new Way([]);
            var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
            maxFlow += maxWayFlow;
            //graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
            residualGraph = graphState.ToResidual();
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private ExcessGraphState SetInitialFlow(ExcessGraphState graphState, int startNode)
    {
        var newGraphState = graphState with {};

        foreach (var adjacentNode in graphState.AdjacencyList[startNode])
        {
            var edgeKey = (startNode, adjacentNode);
            var edge = graphState.Edges[edgeKey];
            newGraphState = newGraphState.AddFlow(edge, edge.Capacity);
        }

        return newGraphState;
    }
}