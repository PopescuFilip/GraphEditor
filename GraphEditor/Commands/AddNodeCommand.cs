using GraphEditor.ViewModels;
using System.Windows;

namespace GraphEditor.Commands;

public class AddNodeCommand(GraphViewModel _vm) : CommandBase<Point>, ICommand<Point>
{
    public override void Execute(Point parameter)
    {
        var nodeNumber = _vm.Nodes.Count + 1;
        var nodeViewModel = new NodeViewModel(nodeNumber, parameter, _vm.OnNodeSelected);
        _vm.Nodes.Add(nodeViewModel);
    }
}