using GraphEditor.Algorithms;
using GraphEditor.Models;

namespace GraphEditor.Commands;

public class AlgorithmCommand(Func<Graph> _getGraph, GenericAlgorithm genericAlgorithm) : CommandBase
{
    public override void Execute(object? parameter)
    {
        var graph = _getGraph();
        var output = genericAlgorithm.Run(graph);
    }
}