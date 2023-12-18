using Advent_of_Code_2023.Utilities;

namespace Advent_of_Code_2023;

public abstract class Day10
{
    public enum IncomingDirection { North, South, None }

    public static void Solve()
    {
        var lines = File.ReadLines("Day10.txt").ToList();

        var grid = new List<List<char>>();
        var start = Point.Origin;
        for (var y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            grid.Add(line.ToList());
            var x = line.IndexOf('S');
            if (x >= 0) start = new Point(x, y);
        }

        ReplaceS(grid, start);

        var visited = new Dictionary<Point, int>();
        var frontier = new Queue<(Point, int)>();
        frontier.Enqueue((start, 0));

        while (frontier.Any())
        {
            var (current, distance) = frontier.Dequeue();
            foreach (var neighbour in Neighbours(grid, current).Where(neighbour => !visited.ContainsKey(neighbour)))
                frontier.Enqueue((neighbour, distance + 1));
            visited[current] = distance;
        }

        Console.WriteLine($"Part1: {visited.Values.Max()}");

        var part2 = 0;

        for (var y = 0; y < grid.Count; y++)
        {
            var inside = false;
            var incomingDirection = IncomingDirection.None;
            for (var x = 0; x < grid[y].Count; x++)
            {
                var point = new Point(x, y);
                if (visited.ContainsKey(point))
                {
                    switch (GetChar(grid, point))
                    {
                        case '|':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            inside = !inside;
                            break;
                        case 'L':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            incomingDirection = IncomingDirection.North;
                            break;
                        case 'F':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            incomingDirection = IncomingDirection.South;
                            break;
                        case '-':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            break;
                        case 'J':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            if (incomingDirection == IncomingDirection.South) inside = !inside;
                            incomingDirection = IncomingDirection.None;
                            break;
                        case '7':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            if (incomingDirection == IncomingDirection.North) inside = !inside;
                            incomingDirection = IncomingDirection.None;
                            break;
                        default:
                            throw new ApplicationException();
                    }
                    continue;
                }

                if (inside) part2++;
            }
        }

        Console.WriteLine($"Part2: {part2}");
    }

    private static List<Point> Neighbours(List<List<char>> grid, Point pos) =>
        GetChar(grid, pos) switch
        {
            '|' => new List<Point> {pos.North, pos.South},
            '-' => new List<Point> {pos.West, pos.East},
            'L' => new List<Point> {pos.North, pos.East},
            'J' => new List<Point> {pos.North, pos.West},
            '7' => new List<Point> {pos.West, pos.South},
            'F' => new List<Point> {pos.East, pos.South},
            _ => throw new ApplicationException()
        };

    private static void ReplaceS(List<List<char>> grid, Point start)
    {
        var ends = new List<Direction>();

        if (GetChar(grid, start.North) is '|' or '7' or 'F') ends.Add(Direction.N);
        if (GetChar(grid, start.South) is '|' or 'L' or 'J') ends.Add(Direction.S);
        if (GetChar(grid, start.West) is '-' or 'L' or 'F') ends.Add(Direction.W);
        if (GetChar(grid, start.East) is '-' or 'J' or '7') ends.Add(Direction.E);

        if (ends.Count != 2) throw new ApplicationException();
        if (ends[0] == Direction.N)
        {
            grid[start.y][start.x] = ends[1] switch
            {
                Direction.S => '|',
                Direction.W => 'J',
                Direction.E => 'L',
                _ => throw new ApplicationException()
            };
            return;
        }

        if (ends[0] == Direction.S)
        {
            grid[start.y][start.x] = ends[1] switch
            {
                Direction.W => '7',
                Direction.E => 'F',
                _ => throw new ApplicationException()
            };
            return;
        }

        if (ends[0] == Direction.W && ends[1] == Direction.E)
        {
            grid[start.y][start.x] = '-';
            return;
        }

        throw new ApplicationException();
    }

    private static char GetChar(List<List<char>> grid, Point pos) => Day03.GetChar(pos.y, pos.x, grid);
}