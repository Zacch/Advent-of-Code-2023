// ReSharper disable AccessToModifiedClosure
namespace Advent_of_Code_2023;

public abstract class Day13
{
    private class Pattern
    {
        public readonly List<string> Rows = new();
        public readonly List<string> Columns = new();

        public void CreateColumns()
        {
            for (var i = 0; i < Rows[0].Length; i++)
            {
                Columns.Add(string.Join("", Rows.Select(r => r.ToCharArray()[i])));
            }
        }
    }
    public static void Solve()
    {
        var lines = File.ReadLines("Day13.txt").ToList();
        var patterns = new List<Pattern>();
        var pattern = new Pattern();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (!pattern.Rows.Any()) continue;
                pattern.CreateColumns();
                patterns.Add(pattern);
                pattern = new Pattern();
            }
            else
                pattern.Rows.Add(line);
        }

        if (pattern.Rows.Any())
        {
            pattern.CreateColumns();
            patterns.Add(pattern);
        }

        Console.WriteLine($"Part1: {Part1(patterns)}");
        Console.WriteLine($"Part2: {Part2(patterns)}");
    }

    private static int Part1(List<Pattern> patterns)
    {
        int columns = 0, rows = 0;
        foreach (var pattern in patterns)
        {
            var reflection = FindReflection(pattern.Rows);
            if (reflection >= 0) rows += reflection;
            else
            {
                reflection = FindReflection(pattern.Columns);
                if (reflection >= 0) columns += reflection;
            }
        }

        return rows * 100 + columns;
    }

    private static int Part2(List<Pattern> patterns)
    {
        int columns = 0, rows = 0;
        foreach (var pattern in patterns)
        {
            var unsmudged = FindSmudgedReflection(pattern.Rows.ToArray());
            if (unsmudged >= 0)
                rows += unsmudged;
            else
            {
                unsmudged = FindSmudgedReflection(pattern.Columns.ToArray());
                if (unsmudged < 0)
                {
                    foreach (var column in pattern.Columns) Console.WriteLine(column);
                    throw new ApplicationException();
                }
                columns += unsmudged;
            }
        }

        return rows * 100 + columns;
    }

    private static int FindSmudgedReflection(string[] lines)
    {
        var unsmudgedReflection = FindReflection(lines.ToList());

        for (var i = 0; i < lines.Count(); i++)
        {
            if (lines[i] == "#...#....##....")
                Console.WriteLine("");
            var original = lines[i].ToCharArray();
            for (var j = 0; j < original.Length; j++)
            {
                var unsmudged = original[..j].ToList();
                unsmudged.Add(original[j] == '.' ? '#' : '.');
                if (j + 1 < original.Length) unsmudged.AddRange(original[(j + 1)..]);
                lines[i] = string.Join("", unsmudged);
                var reflection = FindReflection(lines.ToList(), unsmudgedReflection);
                if (reflection >= 0 && (unsmudgedReflection < 0 || reflection != unsmudgedReflection))
                    return reflection;
            }
            lines[i] = string.Join("", original);
        }

        return -1;
    }

    private static int FindReflection(List<string> lines, int forbidden = -1)
    {
        for (var i = 0; i < lines.Count - 1; i++)
        {
            if (lines[i] != lines[i + 1]) continue;
            int left = i, right = i + 1;
            var match = true;
            while (left >= 0 && right < lines.Count)
            {
                if (lines[left] != lines[right]) match = false;
                left--;
                right++;
            }
            if (match && i + 1 != forbidden) return i + 1;
        }

        return -1;
    }
}

// 9472 is too low