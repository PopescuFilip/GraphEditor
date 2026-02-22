using System.Windows;
using System.Windows.Media;

namespace GraphEditor.ViewModels;

public class EdgeViewModel : ViewModelBase
{
    const double ArrowLength = 30;
    const double ArrowWidth = 10;

    public NodeViewModel StartNode { get; }
    public NodeViewModel EndNode { get; }

    public int Flow { get; }
    public int Capacity { get; }

    public string FlowCapacity => $"{Flow}/{Capacity}";

    public PathGeometry PathGeometry
    {
        get
        {
            var figure = new PathFigure { StartPoint = StartNode.Position };
            figure.Segments.Add(new LineSegment
            {
                Point = EndNode.Position,
                IsStroked = true
            });

            return new PathGeometry([figure]);
        }
    }

    public Geometry ArrowGeometry
    {
        get
        {
            Vector direction = EndNode.Position - StartNode.Position;
            direction.Normalize();
            Vector perpendicular = new Vector(-direction.Y, direction.X);

            var tip = EndNode.Position;
            var left = tip - direction * ArrowLength + perpendicular * (ArrowWidth / 2);
            var right = tip - direction * ArrowLength - perpendicular * (ArrowWidth / 2);

            var streamGeometry = new StreamGeometry();
            using (var ctx = streamGeometry.Open())
            {
                ctx.BeginFigure(tip, true, true);
                ctx.LineTo(left, true, false);
                ctx.LineTo(right, true, false);
            }

            return streamGeometry;
        }
    }

    public Point LabelPosition
    {
        get
        {
            var start = StartNode.Position;
            var end = EndNode.Position;

            var mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            var dir = end - start;
            dir.Normalize();
            var perp = new Vector(-dir.Y, dir.X);

            double offsetDistance = 30;
            return new Point(mid.X + perp.X * offsetDistance, mid.Y + perp.Y * offsetDistance);
        }
    }

    public EdgeViewModel(NodeViewModel startNode, NodeViewModel endNode)
    {
        StartNode = startNode;
        EndNode = endNode;
        Flow = new Random().Next(20);
        Capacity = new Random().Next(20);
    }
}