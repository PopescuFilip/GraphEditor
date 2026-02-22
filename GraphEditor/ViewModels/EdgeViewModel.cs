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
            Point start = StartNode.Position;
            Point end = EndNode.Position;

            // 1. Calculate Midpoint
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            // 2. Get Perpendicular Vector (the "Normal")
            Vector direction = end - start;
            if (direction.Length < 1) return new PathGeometry();

            direction.Normalize();
            Vector normal = new Vector(-direction.Y, direction.X);

            // 3. Offset the control point (e.g., 30 pixels out)
            // This ensures A->B and B->A curve in opposite directions
            double curveOffset = 30;
            Point controlPoint = new Point(mid.X + normal.X * curveOffset,
                                           mid.Y + normal.Y * curveOffset);

            // 4. Create the Bezier Figure
            var figure = new PathFigure { StartPoint = start, IsClosed = false };
            figure.Segments.Add(new QuadraticBezierSegment
            {
                Point1 = controlPoint, // The "pull" point for the curve
                Point2 = end,          // The destination
                IsStroked = true
            });

            return new PathGeometry([figure]);
        }
    }

    public Geometry ArrowGeometry
    {
        get
        {
            Point start = StartNode.Position;
            Point end = EndNode.Position;

            // Re-calculate the same control point used in PathGeometry
            Vector dir = end - start;
            dir.Normalize();
            Vector normal = new Vector(-dir.Y, dir.X);
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            Point controlPoint = new Point(mid.X + normal.X * 30, mid.Y + normal.Y * 30);

            // The tangent at the end of a Quadratic Bezier is the vector from ControlPoint to EndPoint
            Vector tangent = end - controlPoint;
            tangent.Normalize();
            Vector perp = new Vector(-tangent.Y, tangent.X);

            Point tip = end;
            Point left = tip - tangent * ArrowLength + perp * (ArrowWidth / 2);
            Point right = tip - tangent * ArrowLength - perp * (ArrowWidth / 2);

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
            // Re-calculate the control point
            Vector dir = EndNode.Position - StartNode.Position;
            dir.Normalize();
            Vector normal = new Vector(-dir.Y, dir.X);
            Point mid = new Point((StartNode.Position.X + EndNode.Position.X) / 2,
                                  (StartNode.Position.Y + EndNode.Position.Y) / 2);

            // Position the label slightly further out than the curve peak
            double labelOffset = 45;
            return new Point(mid.X + normal.X * labelOffset,
                             mid.Y + normal.Y * labelOffset);
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