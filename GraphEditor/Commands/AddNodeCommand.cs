using GraphEditor.Models;
using GraphEditor.ViewModels;
using System.Windows;

namespace GraphEditor.Commands;

public class AddNodeCommand(MainViewModel _vm) : CommandBase<Point>, ICommand<Point>
{
    public override void Execute(Point parameter)
    {
        var nodeNumber = _vm.Nodes.Count + 1;
        _vm.Nodes.Add(new Node { Number = nodeNumber, Position = parameter });
    }
}