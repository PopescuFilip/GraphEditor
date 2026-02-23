using GraphEditor.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GraphEditor.ViewModels;

public class GraphViewModel : ViewModelBase
{
    private NodeViewModel? firstNode;
    private NodeViewModel? secondNode;

    public ObservableCollection<NodeViewModel> Nodes { get; set; } = [];

    public ObservableCollection<EdgeViewModel> Edges { get; set; } = [];

    public ICommand<Point> AddNodeCommand { get; }
    public ICommand ClearSelection { get; }
    public ICommand Delete { get; }

    public GraphViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
        ClearSelection = new RelayCommand(ClearEdgesSelection);
        Delete = new RelayCommand(DeleteSelected);
    }

    public void OnNodeSelected(NodeViewModel clickedNode)
    {
        clickedNode.IsSelected = !clickedNode.IsSelected;

        if (!clickedNode.IsSelected)
        {
            if (firstNode == null || firstNode != clickedNode)
                return;

            firstNode = null;
            return;
        }

        if (firstNode == null)
        {
            firstNode = clickedNode;
        }
        else
        {
            secondNode ??= clickedNode;
            CreateEdge();
        }
    }

    private void CreateEdge()
    {
        if (firstNode is null || secondNode is null)
            return;

        if (!Edges.Where(x => x.StartNode == firstNode && x.EndNode == secondNode).Any())
        {
            var newEdge = new EdgeViewModel(firstNode, secondNode);
            Edges.Add(newEdge);
        }

        ClearNodeSelection();
    }

    private void ClearEdgesSelection()
    {
        foreach (var edge in Edges)
            edge.IsSelected = false;
    }

    private void ClearNodeSelection()
    {
        if (firstNode != null)
        {
            firstNode.IsSelected = false;
            firstNode = null;
        }
        if (secondNode != null)
        {
            secondNode.IsSelected = false;
            secondNode = null;
        }
    }

    private void DeleteSelected()
    {
        var selectedEdges = Edges.Where(x => x.IsSelected).ToList();
        foreach (var edge in selectedEdges)
            Edges.Remove(edge);
        ClearEdgesSelection();

        var selectedNode = Nodes.SingleOrDefault(x => x.IsSelected);
        if (selectedNode == null)
            return;
        ClearNodeSelection();

        var associatedEdges = Edges.Where(x => x.StartNode == selectedNode || x.EndNode == selectedNode)
            .ToList();
        foreach (var edge in associatedEdges)
            Edges.Remove(edge);

        Nodes.Remove(selectedNode);
    }
}