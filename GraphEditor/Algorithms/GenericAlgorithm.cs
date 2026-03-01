using GraphEditor.Models;

namespace GraphEditor.Algorithms;

public class GenericAlgorithm
{
    public (int MaxFlux, Graph ResultingGraph) Run(Graph graph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0);

    public (int MaxFlux, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow)
    {
        var adjacencyList =  GraphState.CreateRange(graph.Edges);


        return (0, graph with { Edges = [.. adjacencyList.GetEdges()] });
    }
}