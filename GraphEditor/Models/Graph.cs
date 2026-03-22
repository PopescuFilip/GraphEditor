using System.Collections.Immutable;

namespace GraphEditor.Models;

public record Graph<T>(ImmutableArray<Node> Nodes, ImmutableArray<T> Edges) where T : SimpleEdge;

public record Graph(ImmutableArray<Node> Nodes, ImmutableArray<FlowEdge> Edges) : Graph<FlowEdge>(Nodes, Edges);