
using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public abstract class Day08
{
    public static void Solve()
    {
        var lines = File.ReadLines("Day08.txt").ToArray();

        var nodes = new Dictionary<string, (string, string)>();

        var directions = lines[0].ToCharArray();
        foreach (var line in lines.Skip(2))
        {
            var match = Regex.Match(line, "(.+) = \\((.+), (.+)\\)");
            nodes.Add(match.Groups[1].Value, (match.Groups[2].Value, match.Groups[3].Value));
        }

        var part1 = 0;
        var node = "AAA";
        while (node != "ZZZ") node = Move(node, part1++, directions, nodes);
        Console.WriteLine($"Part 1: {part1}");

        var part2Nodes = nodes.Keys.Where(k => k.EndsWith('A')).ToList();
        var periods = new List<ulong>();
        foreach (var part2Node in part2Nodes)
        {
            var node2 = part2Node;
            var i = 0L;
            while (!node2.EndsWith('Z')) node2 = Move(node2, i++, directions, nodes);
            var start = i;

            // Just to be sure...
            while (i == start || !node2.EndsWith('Z')) node2 = Move(node2, i++, directions, nodes);
            var period = i - start;
            if (period != start) throw new ApplicationException("Oh no :P");

            periods.Add((ulong) period);
        }

        var part2 = periods.Aggregate(1UL,
            (current, period) => current * (period / Gcd(period, current)));

        Console.WriteLine($"Part 2: {part2}");
    }

    private static string Move(string node, long i, char[] directions, Dictionary<string, (string, string)> nodes) =>
        directions[i % directions.Length] == 'L' ? nodes[node].Item1 : nodes[node].Item2;

    ///<summary>Thank you, Wikipedia ❤️ (https://en.wikipedia.org/wiki/Euclidean_algorithm)</summary>
    private static ulong Gcd(ulong a, ulong b)
    {
        while (b != 0)
        {
            var oldB = b;
            b = a % b;
            a = oldB;
        }

        return a;
    }
}