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

    public PathGeometry PathGeometry => GetPathGeometry();

    public Geometry ArrowGeometry => GetArrowGeometry();

    public Point LabelPosition => GetLabelPosition();

    public EdgeViewModel(NodeViewModel startNode, NodeViewModel endNode)
    {
        StartNode = startNode;
        EndNode = endNode;
        Flow = new Random().Next(20);
        Capacity = new Random().Next(20);
    }

    private Point GetLabelPosition()
    {
        var dir = EndNode.Position - StartNode.Position;
        dir.Normalize();
        var normal = new Vector(-dir.Y, dir.X);
        var mid = new Point(
            (StartNode.Position.X + EndNode.Position.X) / 2,
            (StartNode.Position.Y + EndNode.Position.Y) / 2);

        double labelOffset = 45;
        return new Point(
            mid.X + normal.X * labelOffset,
            mid.Y + normal.Y * labelOffset);
    }

    private Geometry GetArrowGeometry()
    {
        var start = StartNode.Position;
        var end = EndNode.Position;

        var dir = end - start;
        dir.Normalize();
        var normal = new Vector(-dir.Y, dir.X);
        var mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
        var controlPoint = new Point(mid.X + normal.X * 30, mid.Y + normal.Y * 30);

        var tangent = end - controlPoint;
        tangent.Normalize();
        var perp = new Vector(-tangent.Y, tangent.X);

        var tip = end;
        var left = tip - tangent * ArrowLength + perp * (ArrowWidth / 2);
        var right = tip - tangent * ArrowLength - perp * (ArrowWidth / 2);

        var streamGeometry = new StreamGeometry();
        using (var ctx = streamGeometry.Open())
        {
            ctx.BeginFigure(tip, true, true);
            ctx.LineTo(left, true, false);
            ctx.LineTo(right, true, false);
        }
        return streamGeometry;
    }

    private PathGeometry GetPathGeometry()
    {
        var start = StartNode.Position;
        var end = EndNode.Position;

        var mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

        var direction = end - start;
        if (direction.Length < 1) return new PathGeometry();

        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);

        double curveOffset = 30;
        var controlPoint = new Point(mid.X + normal.X * curveOffset, mid.Y + normal.Y * curveOffset);

        var figure = new PathFigure { StartPoint = start, IsClosed = false };
        figure.Segments.Add(new QuadraticBezierSegment
        {
            Point1 = controlPoint,
            Point2 = end,
            IsStroked = true
        });

        return new PathGeometry([figure]);
    }
}