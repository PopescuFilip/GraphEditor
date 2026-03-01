using GraphEditor.Commands;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphEditor.ViewModels;

public class EdgeViewModel : ViewModelBase
{
    public NodeViewModel StartNode { get; }
    public NodeViewModel EndNode { get; }

    private int _flow;
    public int Flow
    {
        get => _flow;
        set
        {
            _flow = value;
            OnPropertyChanged(nameof(Flow));
            OnPropertyChanged(nameof(FlowCapacity));
        }
    }

    private int _capacity;
    public int Capacity
    {
        get => _capacity;
        set
        {
            _capacity = value;
            OnPropertyChanged(nameof(Capacity));
            OnPropertyChanged(nameof(FlowCapacity));
        }
    }

    public string FlowCapacity => $"{Flow}/{Capacity}";

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

    public ICommand OnClick { get; }

    public EdgeViewModel(NodeViewModel startNode, NodeViewModel endNode)
    {
        StartNode = startNode;
        EndNode = endNode;
        Flow = 0;
        Capacity = 0;
        OnClick = new RelayCommand(() => IsSelected = !IsSelected);
    }
}