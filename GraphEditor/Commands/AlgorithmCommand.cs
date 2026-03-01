using GraphEditor.Algorithms;
using GraphEditor.Models;
using GraphEditor.ViewModels;

namespace GraphEditor.Commands;

public class AlgorithmCommand(Func<Graph> _getGraph, GenericAlgorithm _genericAlgorithm, MainViewModel _vm) : CommandBase
{
    public override void Execute(object? parameter)
    {
        var graph = _getGraph();
        var output = _genericAlgorithm.Run(graph);
        _vm.Navigate<ReadOnlyGraphViewModel>((graphVM) => Configure(graphVM, output));
    }

    private static void Configure(ReadOnlyGraphViewModel graphViewModel, (int MaxFlow, Graph ResultingGraph) algorithmOutput)
    {
        graphViewModel.InitializeFrom(algorithmOutput.ResultingGraph);
        graphViewModel.MaxFlow = algorithmOutput.MaxFlow;
    }
}