using GraphEditor.Models;

namespace GraphEditor.Algorithms;

public class GenericAlgorithm
{
    public (uint MaxFlux, Graph ResultingGraph) Run(Graph graph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number));

    public (uint MaxFlux, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode)
    {

        return (0, graph);
    }
}