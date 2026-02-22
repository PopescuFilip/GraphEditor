using GraphEditor.Commands;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModels;

public class GraphViewModel : ViewModelBase
{
    private NodeViewModel? firstNode;
    private NodeViewModel? secondNode;

    public ObservableCollection<NodeViewModel> Nodes { get; set; } = [];

    public ObservableCollection<EdgeViewModel> Edges { get; set; } = [];

    public ICommand<Point> AddNodeCommand { get; }

    public GraphViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
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

        var newEdge = new EdgeViewModel(firstNode, secondNode);
        Edges.Add(newEdge);

        firstNode.IsSelected = false;
        secondNode.IsSelected = false;
        firstNode = null;
        secondNode = null;
    }
}