using GraphEditor.Algorithms;
using GraphEditor.Models;
using GraphEditor.ViewModels;

namespace GraphEditor.Commands;

public class AlgorithmCommand(Func<Graph> _getGraph, IAlgorithm _algorithm, MainViewModel _vm) : CommandBase
{
    public override void Execute(object? parameter)
    {
        var graph = _getGraph();
        var output = _algorithm.Run(graph);
        _vm.Navigate<ReadOnlyGraphViewModel>((graphVM) => Configure(graphVM, output));
    }

    private static void Configure(ReadOnlyGraphViewModel graphViewModel, (int MaxFlow, Graph ResultingGraph) algorithmOutput)
    {
        graphViewModel.InitializeFrom(algorithmOutput.ResultingGraph);
        graphViewModel.MaxFlow = algorithmOutput.MaxFlow;
    }
}