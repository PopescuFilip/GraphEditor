using GraphEditor.Models;

namespace GraphEditor.Store;

public class ResidualGraphStore
{
    public List<Graph<ResidualEdge>> Graphs { get; } = [];
}