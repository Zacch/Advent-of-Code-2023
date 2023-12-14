namespace Advent_of_Code_2023;

public abstract class Day14
{

    public static void Solve()
    {
        var lines = File.ReadLines("Day14.txt").ToList();

        var grid1 = new List<List<char>>();
        var grid2 = new List<List<char>>();
        foreach (var line in lines)
        {
            grid1.Add(line.ToCharArray().ToList());
            grid2.Add(line.ToCharArray().ToList());
        }

        TiltNorth(grid1);
        Console.WriteLine($"Part1: {LoadOnNorthSupportBeams(grid1)}");

        Console.WriteLine($"Part2: {Part2(grid2)}");
    }

    private static object Part2(List<List<char>> grid)
    {
        var states = new List<string>  {string.Join("", grid.SelectMany(r => r))};
        var loads = new List<int> {LoadOnNorthSupportBeams(grid)};

        for (var i = 1; i < int.MaxValue; i++)
        {
            TiltNorth(grid);
            TiltWest(grid);
            TiltSouth(grid);
            TiltEast(grid);
            var state = string.Join("", grid.SelectMany(r => r));
            var previous = states.IndexOf(state);
            if (previous >= 0)
            {
                var cycle = i - previous;
                var target = previous + (int) ((1000000000L - previous) % cycle);
                return loads[target];
            }
            states.Add(state);
            loads.Add(LoadOnNorthSupportBeams(grid));
        }

        throw new ApplicationException();
    }

    private static void TiltNorth(List<List<char>> grid)
    {
        for (var row = 1; row < grid.Count; row++)
        {
            for (var col = 0; col < grid[row].Count; col++)
            {
                if (Get(row, col, grid) != 'O') continue;
                var destination = row;
                while (Get(destination - 1, col, grid) == '.') destination--;
                Set(row, col, '.', grid);
                Set(destination, col, 'O', grid);
            }
        }
    }

    private static void TiltWest(List<List<char>> grid)
    {
        for (var col = 1; col < grid[0].Count; col++)
        {
            for (var row = 0; row < grid.Count; row++)
            {
                if (Get(row, col, grid) != 'O') continue;
                var destination = col;
                while (Get(row, destination - 1, grid) == '.') destination--;
                Set(row, col, '.', grid);
                Set(row, destination, 'O', grid);
            }
        }
    }

    private static void TiltSouth(List<List<char>> grid)
    {
        for (var row = grid.Count - 2; row >= 0; row--)
        {
            for (var col = 0; col < grid[row].Count; col++)
            {
                if (Get(row, col, grid) != 'O') continue;
                var destination = row;
                while (Get(destination + 1, col, grid) == '.') destination++;
                Set(row, col, '.', grid);
                Set(destination, col, 'O', grid);
            }
        }
    }

    private static void TiltEast(List<List<char>> grid)
    {
        for (var col = grid[0].Count - 2; col >= 0; col--)
        {
            for (var row = 0; row < grid.Count; row++)
            {
                if (Get(row, col, grid) != 'O') continue;
                var destination = col;
                while (Get(row, destination + 1, grid) == '.') destination++;
                Set(row, col, '.', grid);
                Set(row, destination, 'O', grid);
            }
        }
    }

    private static int LoadOnNorthSupportBeams(List<List<char>> grid) =>
        grid.Select((t, row) => (grid.Count - row) * t.Count(c => c == 'O')).Sum();

    private static char Get(int row, int col, List<List<char>> grid)
    {
        if (row < 0 || col < 0) return '#';
        if (row >= grid.Count || col >= grid[row].Count) return '#';
        return grid[row][col];
    }

    private static void Set(int row, int col, char c, List<List<char>> grid)
    {
        if (row < 0 || col < 0 || row >= grid.Count || col >= grid[row].Count)
            throw new ApplicationException();
        grid[row][col] = c;
    }
}