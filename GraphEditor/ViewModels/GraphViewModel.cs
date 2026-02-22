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

    public GraphViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
        ClearSelection = new RelayCommand(ClearEdgesSelection);
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

        firstNode.IsSelected = false;
        secondNode.IsSelected = false;
        firstNode = null;
        secondNode = null;
    }

    private void ClearEdgesSelection()
    {
        foreach (var edge in Edges)
            edge.IsSelected = false;
    }
}