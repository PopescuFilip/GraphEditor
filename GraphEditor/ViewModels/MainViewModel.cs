using GraphEditor.Commands;
using GraphEditor.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string MyString => "valuee";

    public ViewModelBase CurrentViewModel => this;

    public ObservableCollection<Node> Nodes { get; set; } = [];

    public double NodeSize => 30;

    public ICommand<Point> AddNodeCommand { get; }

    public MainViewModel()
    {
        AddNodeCommand = new AddNodeCommand(this);
    }

    public void UpdateCanvas(Canvas canvas)
    {
        canvas.Children.Clear();
        foreach (var node in Nodes)
        {
            var ellipse = new Ellipse
            {
                Width = NodeSize,
                Height = NodeSize,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            var text = new TextBlock
            {
                Text = node.Number.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };

            var nodeGrid = new Grid
            {
                Width = NodeSize,
                Height = NodeSize
            };
            nodeGrid.Children.Add(ellipse);
            nodeGrid.Children.Add(text);

            Canvas.SetLeft(nodeGrid, node.Position.X - NodeSize / 2);
            Canvas.SetTop(nodeGrid, node.Position.Y - NodeSize / 2);

            canvas.Children.Add(nodeGrid);
        }
    }
}