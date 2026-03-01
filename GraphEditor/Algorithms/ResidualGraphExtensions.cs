using GraphEditor.Models;

namespace GraphEditor.Algorithms;

public static class ResidualGraphExtensions
{
    public static int GetMinResidualValue(this GraphState<ResidualEdge> graphState, IEnumerable<(int, int)> edges) =>
        edges.Select(key => graphState.Edges[key].ResidualValue).Min();
}