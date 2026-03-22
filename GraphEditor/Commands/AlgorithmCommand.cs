using GraphEditor.Algorithms;
using GraphEditor.Models;
using GraphEditor.Store;
using GraphEditor.ViewModels;

namespace GraphEditor.Commands;

public class AlgorithmCommand(
    Func<Graph> getGraph,
    IAlgorithm algorithm,
    NavigationCommand<ReadOnlyGraphViewModel> navigationCommand,
    ResidualGraphStore graphStore)
    : CommandBase
{
    public override void Execute(object? parameter)
    {
        var graph = getGraph();

        graphStore.Graphs.Clear();
        var output = algorithm.Run(graph, (graph => graphStore.Graphs.Add(graph)));
        navigationCommand.Execute((graphVM) => Configure(graphVM, output));
    }

    private static void Configure(ReadOnlyGraphViewModel graphViewModel, (int MaxFlow, Graph ResultingGraph) algorithmOutput)
    {
        graphViewModel.InitializeFrom(algorithmOutput.ResultingGraph);
        graphViewModel.MaxFlow = algorithmOutput.MaxFlow;
    }
}