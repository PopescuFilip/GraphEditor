using GraphEditor.Commands;
using System.Windows;
using System.Windows.Input;

namespace GraphEditor.ViewModels;

public class NodeViewModel : ViewModelBase
{
    public int Number { get; }

    private Point _position;
    public Point Position
    {
        get => _position;
        set
        {
            _position = value;
            OnPropertyChanged(nameof(Position));
        }
    }

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

    public ICommand OnClick { get; set; }

    public NodeViewModel(int number, Point position, Action<NodeViewModel> onClick)
    {
        OnClick = new RelayCommand<NodeViewModel>(onClick, this);
        Number = number;
        Position = position;
    }
}