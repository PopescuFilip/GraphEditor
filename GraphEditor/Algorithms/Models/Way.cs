using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Models;

public record Way(ImmutableArray<(int StartNode, int EndNode)> Edges);