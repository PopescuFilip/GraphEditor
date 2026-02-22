using GraphEditor.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphEditor.Views;

public class CanvasNodeView : Grid
{
    private const int NodeSize = 20;

    private readonly Node _node;

    public CanvasNodeView(Node node)
    {
        _node = node;
        Width = NodeSize;
        Height = NodeSize;

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

        Children.Add(ellipse);
        Children.Add(text);
        Canvas.SetLeft(this, node.Position.X - NodeSize / 2);
        Canvas.SetTop(this, node.Position.Y - NodeSize / 2);
    }
}