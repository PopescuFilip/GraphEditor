using GraphEditor.Models;

namespace GraphEditor.Algorithms;

public interface IAlgorithm
{
    (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph);
}