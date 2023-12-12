namespace Advent_of_Code_2023;

public abstract class Day12
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day12.txt").ToList();
        var part1 = 0L;
        var part2 = 0L;
        foreach (var line in lines)
        {
            var parts = line.Split(' ');

            var springs = parts[0].ToCharArray();
            var records = parts[1].Split(',').Select(long.Parse).ToList();
            part1 += CountPossibleArrangements(springs, records, new Dictionary<string, long>());

            var unfoldedSprings = $"{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}".ToCharArray();
            var unfoldedRecords = $"{parts[1]},{parts[1]},{parts[1]},{parts[1]},{parts[1]}"
                .Split(',').Select(long.Parse).ToList();
            part2 += CountPossibleArrangements(unfoldedSprings, unfoldedRecords, new Dictionary<string, long>());
        }
        Console.WriteLine($"Part1: {part1}");
        Console.WriteLine($"Part2: {part2}");
    }

    private static long CountPossibleArrangements(char[] springs, List<long> records, Dictionary<string, long> solutions)
    {
        var key = MakeKey(springs, records);
        if (solutions.TryGetValue(key, out var solution)) return solution;

        var i = 0L;
        while (i < springs.Length && springs[i] == '.') i++;
        if (i == springs.Length) return records.Any() ? 0 : 1;

        if (springs[i] == '#')
        {
            if (!records.Any() || i + records[0] > springs.Length) return 0;
            for (var j = 0; j < records[0]; j++) if (springs[i + j] == '.') return 0;
            i += records[0];
            var remainingRecords = records.Skip(1).ToList();
            if (i < springs.Length && springs[i] == '#') return 0;
            i++;
            if (i >= springs.Length) return remainingRecords.Any() ? 0 : 1;
            var remainingSprings = springs[(int)i..];
            var result = CountPossibleArrangements(remainingSprings, remainingRecords, solutions);
            solutions[key] = result;
            return result;
        }

        // Springs[i] == '?'
        var remainingSprings2 = new char[springs.Length - i];
        Array.Copy(springs, i, remainingSprings2, 0, springs.Length - i);

        remainingSprings2[0] = '.';
        var operational = CountPossibleArrangements(remainingSprings2, records, solutions);
        remainingSprings2[0] = '#';
        var damaged = CountPossibleArrangements(remainingSprings2, records, solutions);
        solutions[key] = operational + damaged;
        return operational + damaged;
    }

    private static string MakeKey(char[] springs, List<long> records) =>
        $"{string.Join("", springs)} {string.Join(",", records)}";
}