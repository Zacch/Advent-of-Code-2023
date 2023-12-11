using Advent_of_Code_2023.Utilities;
// ReSharper disable SimplifyLinqExpressionUseAll

namespace Advent_of_Code_2023;

public abstract class Day11
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day11.txt").ToList();

        var grid = new List<List<char>>();
        foreach (var line in lines) grid.Add(line.ToList());

        var stars = new List<Point>();

        for (var y = 0; y < grid.Count; y++)
        for (var x = 0; x < grid[y].Count; x++)
            if (Day03.GetChar(y, x, grid) == '#') stars.Add(new Point(x, y));

        var part1Stars = ExpandSpace(stars, 1);

        var part1 = 0L;

        for (var i = 0; i < part1Stars.Count; i++)
        for (var j = i; j < part1Stars.Count; j++)
            part1 += part1Stars[i].ManhattanDistanceTo(part1Stars[j]);
        Console.WriteLine($"Part1: {part1}");

        var part2Stars = ExpandSpace(stars, 999999);

        var part2 = 0L;

        for (var i = 0; i < part2Stars.Count; i++)
        for (var j = i; j < part2Stars.Count; j++)
            part2 += part2Stars[i].ManhattanDistanceTo(part2Stars[j]);

        Console.WriteLine($"Part1: {part2}");
    }

    private static List<Point> ExpandSpace(List<Point> stars, int rate)
    {
        for (var y = stars.Max(s => s.y) - 1; y >= 0; y--)
            if (!stars.Any(s => s.y == y))
                stars = stars.Select(s => s.y > y ? new Point(s.x, s.y + rate) : s).ToList();

        for (var x = stars.Max(s => s.x) - 1; x >= 0; x--)
            if (!stars.Any(s => s.x == x))
                stars = stars.Select(s => s.x > x ? new Point(s.x + rate, s.y) : s).ToList();
        return stars;
    }
}