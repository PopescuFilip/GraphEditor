using GraphEditor.Models;

namespace GraphEditor.Algorithms.Models;

public static class ResidualGraphExtensions
{
    public static int GetMinResidualValue(this GraphState<ResidualEdge> graphState, Way way) =>
        way.Edges.Select(key => graphState.Edges[key].ResidualValue).Min();
}