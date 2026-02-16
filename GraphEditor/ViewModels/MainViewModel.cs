using GraphEditor.Commands;
using GraphEditor.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    public const double NodeSize = 30;

    public string MyString => "valuee";

    public ViewModelBase CurrentViewModel => this;

    public ObservableCollection<Node> Nodes { get; set; } = [];

    public ICommand<Point> AddNodeCommand { get; }

    public MainViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
    }
}