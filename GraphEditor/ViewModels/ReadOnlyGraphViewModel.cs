using GraphEditor.Models;
using System.Collections.ObjectModel;

namespace GraphEditor.ViewModels;

public class ReadOnlyGraphViewModel : ViewModelBase
{
    public int MaxFlow { get; set; }

    public ObservableCollection<Node> Nodes { get; set; } = [];

    public ObservableCollection<ReadOnlyEdgeViewModel> Edges { get; set; } = [];

    public void InitializeFrom(Graph graph)
    {
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

            var edgeVM = new ReadOnlyEdgeViewModel(startNode, endNode, $"{edge.Flow}/{edge.Capacity}");
            Edges.Add(edgeVM);
        }
    }
}