using System.Collections.Immutable;

namespace GraphEditor.Models;

public record Graph(ImmutableArray<Node> Nodes, ImmutableArray<FlowEdge> Edges);