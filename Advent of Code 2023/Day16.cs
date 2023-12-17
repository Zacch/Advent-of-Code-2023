namespace Advent_of_Code_2023;

public abstract class Day16
{
    private enum Direction {  N, W, E, S }
    private class Location
    {
        public readonly char Content;
        public bool N, W, E, S;

        public bool IsEnergized => N || W || E || S;

        public Location(char c) => Content = c;

        public void Clear() { N = false; W = false; E = false; S = false; }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day16.txt").ToList();

        var grid = new List<List<Location>>();
        foreach (var line in lines) grid.Add(line.Select(c => new Location(c)).ToList());
        var part1 = Beam(0, 0, Direction.E, grid);
        Console.WriteLine($"Part1: {part1}");

        var part2 = 0;
        for (var row = 0; row < grid.Count; row++)
        {
            Clear(grid);
            part2 = Math.Max(part2, Beam(row, 0, Direction.E, grid));
            Clear(grid);
            part2 = Math.Max(part2, Beam(row, grid[row].Count - 1, Direction.W, grid));
        }
        for (var col = 0; col < grid[0].Count; col++)
        {
            Clear(grid);
            part2 = Math.Max(part2, Beam(0, col, Direction.S, grid));
            Clear(grid);
            part2 = Math.Max(part2, Beam(grid.Count - 1, grid[0].Count - 1, Direction.W, grid));
        }
        Console.WriteLine($"Part2: {part2}");
    }

    private static void Clear(List<List<Location>> grid)
    { foreach (var location in grid.SelectMany(l => l)) location.Clear(); }

    private static int Beam(int row, int col, Direction direction, List<List<Location>> grid)
    {
        var done = false;
        while (!done)
        {
            var location = grid[row][col];
            switch (direction)
            {
                case Direction.N:
                    if (location.N) { done = true; break; }
                    location.N = true;

                    switch (location.Content)
                    {
                        case '-':
                            if (col == 0) direction = Direction.E;
                            else
                            {
                                direction = Direction.W;
                                if (col + 1 < grid[row].Count) Beam(row, col + 1, Direction.E, grid);
                            }
                            break;
                        case '/':
                            direction = Direction.E;
                            break;
                        case '\\':
                            direction = Direction.W;
                            break;
                    }
                    break;
                case Direction.W:
                    if (location.W) { done = true; break; }
                    location.W = true;

                    switch (location.Content)
                    {
                        case '|':
                            if (row == 0) direction = Direction.S;
                            else
                            {
                                direction = Direction.N;
                                if (row + 1 < grid.Count) Beam(row + 1, col, Direction.S, grid);
                            }
                            break;
                        case '/':
                            direction = Direction.S;
                            break;
                        case '\\':
                            direction = Direction.N;
                            break;
                    }
                    break;
                case Direction.E:
                    if (location.E) { done = true; break; }
                    location.E = true;

                    switch (location.Content)
                    {
                        case '|':
                            if (row == 0) direction = Direction.S;
                            else
                            {
                                direction = Direction.N;
                                if (row + 1 < grid.Count) Beam(row + 1, col, Direction.S, grid);
                            }
                            break;
                        case '/':
                            direction = Direction.N;
                            break;
                        case '\\':
                            direction = Direction.S;
                            break;
                    }
                    break;
                case Direction.S:
                    if (location.S) { done = true; break; }
                    location.S = true;

                    switch (location.Content)
                    {
                        case '-':
                            if (col == 0) direction = Direction.E;
                            else
                            {
                                direction = Direction.W;
                                if (col + 1 < grid[row].Count) Beam(row, col + 1, Direction.E, grid);
                            }
                            break;
                        case '/':
                            direction = Direction.W;
                            break;
                        case '\\':
                            direction = Direction.E;
                            break;
                    }
                    break;
            }

            switch (direction)
            {
                case Direction.N:
                    row--;
                    if (row < 0) done = true;
                    break;
                case Direction.W:
                    col--;
                    if (col < 0) done = true;
                    break;
                case Direction.E:
                    col++;
                    if (col >= grid[row].Count) done = true;
                    break;
                case Direction.S:
                    row++;
                    if (row >= grid.Count) done = true;
                    break;
            }

        }
        return grid.SelectMany(l => l).Count(l => l.IsEnergized);
    }

}