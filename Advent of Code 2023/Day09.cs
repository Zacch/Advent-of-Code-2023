namespace Advent_of_Code_2023;

public abstract class Day09
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day09.txt").ToList();
        var part1 = 0;
        var part2 = 0;
        foreach (var line in lines)
        {
            var sequence = line
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();
            var sequences = new List<List<int>> {sequence};
            while (sequences.Last().Any(n => n != 0))
            {
                var previous = sequences.Last();
                var next = new List<int>();
                for (var i = 0; i < previous.Count - 1; i++)
                {
                    next.Add(previous[i + 1] - previous[i]);
                }
                sequences.Add(next);
            }

            var newFirstNumber = 0;
            for (var i = sequences.Count - 1; i >= 0; i--)
            {
                part1 += sequences[i].Last();
                newFirstNumber = sequences[i].First() - newFirstNumber;
            }

            part2 += newFirstNumber;
        }

        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
    }
}