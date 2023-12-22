// ReSharper disable InconsistentNaming
namespace Advent_of_Code_2023.Utilities;

///<summary>A 3D point in a coordinate system with the Y axis pointing downwards</summary>
public class Point3 : IEquatable<Point3>
{
    public static Point3 Origin { get; } = new(0, 0, 0);

    public readonly int x;
    public readonly int y;
    public readonly int z;

    public Point3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point3 North => new(x, y - 1, z);
    public Point3 South => new(x, y + 1, z);
    public Point3 West => new(x - 1, y, z);
    public Point3 East => new(x + 1, y, z);

    #region Equality

    public static bool operator ==(Point3? left, Point3? right) => Equals(left, right);
    public static bool operator !=(Point3? left, Point3? right) => !Equals(left, right);

    public bool Equals(Point3? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Point3) obj);
    }

    public override int GetHashCode() => HashCode.Combine(x, y, z);

    #endregion

    public override string ToString() => $"({nameof(x)}: {x}, {nameof(y)}: {y}, {nameof(z)}: {z})";

    public int ManhattanDistanceTo(Point3 other) =>
        Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z);
}