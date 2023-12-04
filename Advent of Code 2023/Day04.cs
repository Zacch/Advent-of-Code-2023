namespace Advent_of_Code_2023;

public abstract class Day04
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day04.txt").ToArray();

        var matches = new int[lines.Length];
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var card = line.Split(":")[1].Split("|").ToList();
            var winning = card[0].Split(" ").Where(s => s.Length > 0).Select(int.Parse).ToHashSet();
            var mine = card[1].Split(" ").Where(s => s.Length > 0).Select(int.Parse).ToList();
            matches[i] = mine.Count(n => winning.Contains(n));
        }

        var part1 = matches.Select(m => (int) Math.Pow(2, m - 1)).Sum();
        Console.WriteLine($"Part 1: {part1}");


        var cards = Enumerable.Repeat(1, matches.Length).ToArray();

        for (var i = 0; i < cards.Length; i++)
        {
            for (var j = i + 1; j <= i + matches[i] && j < cards.Length; j++) cards[j] += cards[i];
        }
        var part2 = cards.Sum();

        Console.WriteLine($"Part 2: {part2}");
    }
}