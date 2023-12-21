using Advent_of_Code_2023.Utilities;
// ReSharper disable MergeIntoPattern

namespace Advent_of_Code_2023;

public abstract class Day21
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day21.txt").ToList();
        var grid = new List<List<char>>();

        var start = Point.Origin;
        for (var y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            grid.Add(line.ToList());
            var x = line.IndexOf('S');
            if (x >= 0) start = new Point(x, y);
        }

        grid[start.y][start.x] = '.';

        var visited = new Dictionary<Point, int>();
        var history = new HashSet<Point>();
        var frontier = new Queue<(Point, int)>();
        frontier.Enqueue((start, 0));

        const int pathLength = 64;
        while (frontier.Any())
        {
            var (current, distance) = frontier.Dequeue();
            foreach (var neighbour in Neighbours2(grid, current).Where(n => !history.Contains(n)))
            {
                history.Add(neighbour);
                if (distance < pathLength) frontier.Enqueue((neighbour, distance + 2));
            }
            visited[current] = distance;
        }

        Console.WriteLine($"Part1: {visited.Count}");
    }

    private static List<Point> Neighbours2(List<List<char>> grid, Point current)
    {
        var neighbours = new List<Point>();
        var one = current.North;
        if (grid[one.y][one.x] == '.')
            foreach (var two in new List<Point> {one.North, one.East, one.West})
                if (Get(grid, two) == '.') neighbours.Add(two);

        one = current.West;
        if (Get(grid, one) == '.')
            foreach (var two in new List<Point> {one.North, one.South, one.West})
                if (Get(grid, two) == '.' && !neighbours.Contains(two)) neighbours.Add(two);

        one = current.East;
        if (grid[one.y][one.x] == '.')
            foreach (var two in new List<Point> {one.North, one.South, one.East})
                if (Get(grid, two) == '.' && !neighbours.Contains(two)) neighbours.Add(two);

        one = current.South;
        if (grid[one.y][one.x] == '.')
            foreach (var two in new List<Point> {one.East, one.South, one.West})
                if (Get(grid, two) == '.' && !neighbours.Contains(two)) neighbours.Add(two);

        return neighbours;
    }

    private static char Get(List<List<char>> grid, Point point) =>
        point.x >= 0 && point.y >= 0 && point.y < grid.Count && point.x < grid[point.y].Count ?
            grid[point.y][point.x] : '#';
}