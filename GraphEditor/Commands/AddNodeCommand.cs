using GraphEditor.ViewModels;
using System.Windows;

namespace GraphEditor.Commands;

public class AddNodeCommand(GraphViewModel _vm) : CommandBase<Point>, ICommand<Point>
{
    public override void Execute(Point parameter)
    {
        var nodeNumber = GetAvailableNumber(_vm.Nodes);
        var nodeViewModel = new NodeViewModel(nodeNumber, parameter, _vm.OnNodeSelected);
        _vm.Nodes.Add(nodeViewModel);
    }

    private static int GetAvailableNumber(IEnumerable<NodeViewModel> nodes)
    {
        var availableNumber = 1;
        while (nodes.Select(node => node.Number).Contains(availableNumber))
            availableNumber++;
        return availableNumber;
    }
}