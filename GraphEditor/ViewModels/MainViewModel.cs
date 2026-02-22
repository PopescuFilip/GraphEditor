using GraphEditor.Commands;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ViewModelBase CurrentViewModel => this;

    public ObservableCollection<NodeViewModel> Nodes { get; set; } = [];

    public ICommand<Point> AddNodeCommand { get; }

    public MainViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
    }

    public void OnNodeSelected(NodeViewModel nodeViewModel)
    {
        nodeViewModel.IsSelected = !nodeViewModel.IsSelected;
    }
}