namespace Advent_of_Code_2023;

public abstract class Day03
{
    private class Part
    {
        public readonly int Number;
        public readonly int Row;
        public readonly int Col;
        public readonly char Symbol;

        public Part(int number, int row, int col, char symbol)
        { Number = number; Row = row; Col = col; Symbol = symbol; }
    }
    public static void Solve()
    {
        var lines = File.ReadLines("Day03.txt").ToList();

        var grid = new List<List<char>>();
        foreach (var line in lines) grid.Add(line.ToList());

        var parts = FindParts(grid);
        var part1 = parts.Where(p =>p.Symbol != '.').Select(p => p.Number).Sum();
        Console.WriteLine($"Part 1: {part1}");

        var gears = new Dictionary<string, List<Part>>();
        foreach (var part in parts.Where(p => p.Symbol == '*'))
        {
            var coords = $"({part.Row}, {part.Col})";
            if (!gears.ContainsKey(coords)) gears[coords] = new List<Part>();
            gears[coords].Add(part);
        }
        var part2 = gears.Values.Where(l => l.Count == 2).Select(l => l[0].Number * l[1].Number).Sum();

        Console.WriteLine($"Part 2: {part2}");
    }

    private static List<Part> FindParts(List<List<char>> grid)
    {
        var parts = new List<Part>();
        for (var row = 0; row < grid.Count; row++)
        {
            var currentNumber = 0;
            for (var col = 0; col <= grid[row].Count; col++)
            {
                var c = GetChar(row, col, grid);
                if (char.IsDigit(c))
                {
                    currentNumber = currentNumber * 10 + (c - '0');
                }
                else if (currentNumber > 0)
                {
                    parts.Add(MakePart(currentNumber, row, col, grid));
                    currentNumber = 0;
                }
            }
        }
        return parts;
    }

    private static char GetChar(int row, int col, List<List<char>> grid)
    {
        if (row < 0 || col < 0) return '.';
        if (row >= grid.Count || col >= grid[row].Count) return '.';
        return grid[row][col];
    }

    private static Part MakePart(int partNumber, int row, int columnAfterEnd, List<List<char>> grid)
    {
        var startCol = columnAfterEnd - partNumber.ToString().Length;
        var symbol = GetChar(row, startCol - 1, grid);
        if (symbol != '.') return new Part(partNumber, row, startCol - 1, symbol);
        symbol = GetChar(row, columnAfterEnd, grid);
        if (symbol != '.') return new Part(partNumber, row, columnAfterEnd, symbol);

        for (var col = startCol - 1; col <= columnAfterEnd; col++)
        {
            symbol = GetChar(row - 1, col, grid);
            if (symbol != '.') return new Part(partNumber, row - 1, col, symbol);
            symbol = GetChar(row + 1, col, grid);
            if (symbol != '.') return new Part(partNumber, row + 1, col, symbol);
        }
        return new Part(partNumber, row, startCol, '.');
    }
}