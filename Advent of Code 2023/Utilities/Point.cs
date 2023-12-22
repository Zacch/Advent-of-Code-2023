// ReSharper disable InconsistentNaming
namespace Advent_of_Code_2023.Utilities;

///<summary>A point in a coordinate system with the Y axis pointing downwards</summary>
public class Point : IEquatable<Point>
{
    public static Point Origin { get; } = new(0, 0);

    public readonly int x;
    public readonly int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point North => new(x, y - 1);
    public Point South => new(x, y + 1);
    public Point West => new(x - 1, y);
    public Point East => new(x + 1, y);

    #region Equality

    public static bool operator ==(Point? left, Point? right) => Equals(left, right);
    public static bool operator !=(Point? left, Point? right) => !Equals(left, right);

    public bool Equals(Point? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return x == other.x && y == other.y;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Point) obj);
    }

    public override int GetHashCode() => HashCode.Combine(x, y);

    #endregion

    public override string ToString() => $"({nameof(x)}: {x}, {nameof(y)}: {y})";

    public int ManhattanDistanceTo(Point other) => Math.Abs(x - other.x) + Math.Abs(y - other.y);
}