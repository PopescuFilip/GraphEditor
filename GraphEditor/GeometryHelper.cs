using System.Windows;
using System.Windows.Media;

namespace GraphEditor;

public static class GeometryHelper
{
    const double ArrowLength = 30;
    const double ArrowWidth = 10;

    public static PathGeometry GetEdgePathGeometry(Point start, Point end)
    {
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

    public static Geometry GetEdgeArrowGeometry(Point start, Point end)
    {
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

    public static Point GetEdgeLabelPosition(Point start, Point end)
    {
        var dir = end - start;
        dir.Normalize();
        var normal = new Vector(-dir.Y, dir.X);
        var mid = new Point(
            (start.X + end.X) / 2,
            (start.Y + end.Y) / 2);

        double labelOffset = 45;
        return new Point(
            mid.X + normal.X * labelOffset,
            mid.Y + normal.Y * labelOffset);
    }
}