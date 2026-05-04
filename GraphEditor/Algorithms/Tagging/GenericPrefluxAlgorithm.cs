using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms.Tagging;

public class GenericPrefluxAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var maxFlow = initialFlow;
        var excessGraphState = GraphState.CreateRange(graph.Edges.Select(e => new ExcessEdge(e)));
        var graphState = GraphState.CreateRange(graph.Edges);
        var residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var tags = residualGraph.GetDistanceTags(endNode);
        tags[startNode] = graph.Nodes.Length;
        graphState = graphState with { Edges = graphState.Edges.SetItems(GetInitialEdges(graphState, startNode)) };

        while (true)
        {
            var wayToEndNode = new Way([]);
            var maxWayFlow = residualGraph.GetMinResidualValue(wayToEndNode);
            maxFlow += maxWayFlow;
            graphState = graphState.AddFlow(wayToEndNode, maxWayFlow);
            residualGraph = graphState.ToResidual();
            onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));
        }

        return (maxFlow, graph with { Edges = [.. graphState.GetEdges()] });
    }

    private IEnumerable<KeyValuePair<(int, int), FlowEdge>> GetInitialEdges(GraphState<FlowEdge> graphState, int startNode)
    {
        foreach (var adjacentNode in graphState.AdjacencyList[startNode])
        {
            var edgeKey = (startNode, adjacentNode);
            var edgeValue = graphState.Edges[edgeKey];
            var newEdgeValue = edgeValue with { Flow = edgeValue.Capacity };
            yield return KeyValuePair.Create(edgeKey, newEdgeValue);
        }
    }
}