using GraphEditor.Commands;
using GraphEditor.Models;
using GraphEditor.Store;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GraphEditor.ViewModels;

public class GraphHistoryViewModel : ViewModelBase
{
    private readonly ResidualGraphStore _graphStore;

    private int currentIndex = 0;
    private int CurrentIndex
    {
        get => currentIndex;
        set
        {
            currentIndex = value;
            Update();
            OnPropertyChanged(nameof(CurrentIndex));
        }
    }

    public ObservableCollection<Node> Nodes { get; set; } = [];

    public ObservableCollection<ReadOnlyEdgeViewModel> Edges { get; set; } = [];

    public RelayCommand PreviousCommand { get; }
    public RelayCommand NextCommand { get; }

    public GraphHistoryViewModel(ResidualGraphStore graphStore)
    {
        _graphStore = graphStore;

        PreviousCommand = new RelayCommand(
            () => CurrentIndex--,
            () => CurrentIndex != 0);
        NextCommand = new RelayCommand(
            () => CurrentIndex++,
            () => CurrentIndex != _graphStore.Graphs.Count - 1);
        PropertyChanged += CurrentIndexHandler;
    }

    public void Reset() => CurrentIndex = _graphStore.Graphs.Count - 1;

    private void Update()
    {
        var graph = _graphStore.Graphs[CurrentIndex];
        Edges.Clear();
        Nodes.Clear();

        foreach (var node in graph.Nodes)
        {
            Nodes.Add(node);
        }
        foreach (var edge in graph.Edges)
        {
            var startNode = Nodes.First(n => n.Number == edge.StartNode);
            var endNode = Nodes.First(n => n.Number == edge.EndNode);

            var edgeVM = new ReadOnlyEdgeViewModel(startNode, endNode, $"{edge.ResidualValue}");
            Edges.Add(edgeVM);
        }
    }

    private void CurrentIndexHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentIndex))
        {
            PreviousCommand.OnCanExecutedChanged();
            NextCommand.OnCanExecutedChanged();
        }
    }
}