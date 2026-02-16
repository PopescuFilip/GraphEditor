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
        DrawPoints(vm!.Nodes.Select(p => p.Position));
    }

    private void DrawPoints(IEnumerable<Point> points)
    {
        foreach (var point in points)
        {
            Ellipse ellipse = new()
            {
                Width = NodeSize,
                Height = NodeSize,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(ellipse, point.X - NodeSize / 2);
            Canvas.SetTop(ellipse, point.Y - NodeSize / 2);
            PointsCanvas.Children.Add(ellipse);
        }
    }
}