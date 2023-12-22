using System.Text.RegularExpressions;
using Advent_of_Code_2023.Utilities;
// ReSharper disable InconsistentNaming

namespace Advent_of_Code_2023;

public abstract partial class Day22
{
    [GeneratedRegex(@"(\d+),(\d+),(\d+)~(\d+),(\d+),(\d+)")]
    private static partial Regex InputRegex();

    private class Brick
    {
        public static readonly Brick None = new(Point3.Origin, Point3.Origin);
        private static int nextId;
        private readonly int id = nextId++;

        public Point3 a;
        public Point3 b;

        public Brick(Point3 a, Point3 b) { this.a = a; this.b = b; }

        public override bool Equals(object? obj) => !ReferenceEquals(null, obj) && ReferenceEquals(this, obj);
        public override int GetHashCode() => id;
        public static bool operator ==(Brick? left, Brick? right) => Equals(left, right);
        public static bool operator !=(Brick? left, Brick? right) => !Equals(left, right);
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day22.txt").ToList();

        var bricks = new List<Brick>();

        foreach (var line in lines)
        {
            var match = InputRegex().Match(line);
            var groups = match.Groups;
            var a = new Point3(int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
            var b = new Point3(int.Parse(groups[4].Value), int.Parse(groups[5].Value), int.Parse(groups[6].Value));
            if (a.x > b.x || a.y > b.y || a.z > b.z) (a, b) = (b, a);
            bricks.Add(new Brick(a, b));
        }
        var landed = Land(bricks);

        Console.WriteLine($"Part1: {Part1(landed)}");
        Console.WriteLine($"Part2: {Part2(landed)}");
    }


    private static List<Brick> Land(List<Brick> originalBricks)
    {
        var bricks = new List<Brick>(originalBricks);

        var (dimensions, space) = MakeSpace(bricks);

        var stillFalling = true;
        while (stillFalling)
        {
            stillFalling = false;

            Fill(space, bricks, dimensions);

            foreach (var brick in bricks)
            {
                var canFall = brick.a.z > 1;
                if (canFall)
                    for (var x = brick.a.x; x <= brick.b.x; x++)
                    for (var y = brick.a.y; y <= brick.b.y; y++)
                        if (space[x, y, brick.a.z - 1] != Brick.None) canFall = false;

                if (!canFall) continue;
                stillFalling = true;
                brick.a = new Point3(brick.a.x, brick.a.y, brick.a.z - 1);
                brick.b = new Point3(brick.b.x, brick.b.y, brick.b.z - 1);
            }
        }

        return bricks;
    }

    private static (Point3, Brick[,,]) MakeSpace(List<Brick> bricks)
    {
        var dimensions = new Point3(
            Math.Min(bricks.Max(b => b.a.x), bricks.Max(b => b.b.x)) + 1,
            Math.Min(bricks.Max(b => b.a.y), bricks.Max(b => b.b.y)) + 1,
            Math.Min(bricks.Max(b => b.a.z), bricks.Max(b => b.b.z)) + 1);
        var space = new Brick[dimensions.x + 1, dimensions.y + 1, dimensions.z + 1];
        return (dimensions, space);
    }


    private static void Fill(Brick[,,] space, List<Brick> bricks, Point3 dimensions)
    {
        for (var x = 0; x < dimensions.x; x++)
        for (var y = 0; y < dimensions.y; y++)
        for (var z = 0; z < dimensions.z; z++)
            space[x,y,z] = Brick.None;

        foreach (var brick in bricks)
            for (var x = brick.a.x; x <= brick.b.x; x++)
            for (var y = brick.a.y; y <= brick.b.y; y++)
            for (var z = brick.a.z; z <= brick.b.z; z++)
                space[x, y, z] = brick;
    }

    private static int Part1(List<Brick> bricks)
    {
        var part1 = 0;
        var (dimensions, space) = MakeSpace(bricks);
        Fill(space, bricks, dimensions);

        foreach (var disintegrated in bricks)
        {
            var canBeDisintegrated = true;
            foreach (var brick in bricks.Where(b => b != disintegrated && b.a.z > 1))
            {
                var restsOnAnotherBrick = false;
                for (var x = brick.a.x; x <= brick.b.x; x++)
                for (var y = brick.a.y; y <= brick.b.y; y++)
                    if (space[x, y, brick.a.z - 1] != Brick.None && space[x, y, brick.a.z - 1] != disintegrated)
                        restsOnAnotherBrick = true;
                if (restsOnAnotherBrick) continue;

                canBeDisintegrated = false;
                break;
            }

            if (canBeDisintegrated) part1++;
        }

        return part1;
    }

    private static int Part2(List<Brick> allBricks)
    {
        var part2 = 0;
        var (dimensions, space) = MakeSpace(allBricks);

        foreach (var disintegrated in allBricks)
        {
            var bricks = new List<Brick>(allBricks.Where(b => b != disintegrated).Select(b => new Brick(b.a, b.b)));
            var fallingBricks = new HashSet<Brick>();

            var stillFalling = true;
            while (stillFalling)
            {
                stillFalling = false;

                Fill(space, bricks, dimensions);

                foreach (var brick in bricks)
                {
                    var canFall = brick.a.z > 1;
                    if (canFall)
                        for (var x = brick.a.x; x <= brick.b.x; x++)
                        for (var y = brick.a.y; y <= brick.b.y; y++)
                            if (space[x, y, brick.a.z - 1] != Brick.None)
                                canFall = false;
                    if (!canFall) continue;

                    fallingBricks.Add(brick);
                    stillFalling = true;
                    brick.a = new Point3(brick.a.x, brick.a.y, brick.a.z - 1);
                    brick.b = new Point3(brick.b.x, brick.b.y, brick.b.z - 1);
                }
            }

            part2 += fallingBricks.Count;
        }

        return part2;
    }
}