using GraphEditor.Commands;
using GraphEditor.Models;
using System.Windows;
using System.Windows.Media;

namespace GraphEditor.ViewModels;

public class ReadOnlyEdgeViewModel(Node startNode, Node endNode, string label) : ViewModelBase
{
    public Node StartNode { get; } = startNode;
    public Node EndNode { get; } = endNode;

    public string Label => label;

    public string EdgeName => $"{StartNode.Number} -> {EndNode.Number}";

    public PathGeometry PathGeometry => GeometryHelper.GetEdgePathGeometry(StartNode.Position, EndNode.Position);

    public Geometry ArrowGeometry => GeometryHelper.GetEdgeArrowGeometry(StartNode.Position, EndNode.Position);

    public Point LabelPosition => GeometryHelper.GetEdgeLabelPosition(StartNode.Position, EndNode.Position);

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}