using GraphEditor.Models;
using GraphEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : UserControl
{
    private const double NodeSize = 25;

    public MainView()
    {
        InitializeComponent();
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        var clickPosition = e.GetPosition(sender as UIElement);
        vm?.AddNodeCommand.Execute(clickPosition);
        DrawPoints(vm!.Nodes);
    }

    private void DrawPoints(IEnumerable<Node> nodes)
    {
        foreach (var node in nodes)
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

            PointsCanvas.Children.Add(nodeGrid);
        }
    }
}