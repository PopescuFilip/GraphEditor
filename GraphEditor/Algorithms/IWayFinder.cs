using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Diagnostics.CodeAnalysis;

namespace GraphEditor.Algorithms;

public interface IWayFinder
{
    bool TryFindWayToEndNode(GraphState<ResidualEdge> graphState, [NotNullWhen(true)] out Way? wayToEndNode);
}