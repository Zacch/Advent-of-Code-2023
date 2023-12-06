// ReSharper disable LoopCanBeConvertedToQuery
namespace Advent_of_Code_2023;

public abstract class Day06
{
    private class Race
    {
        private readonly long time;
        private readonly long distance;

        public Race(long time, long distance) { this.time = time; this.distance = distance; }

        public long WaysToWin()
        {
            var wins = 0;
            for (var i = 0; i < time; i++)
                if (i * (time - i) > distance) wins++;
            return wins;
        }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day06.txt").ToArray();

        var times = lines[0].Split(' ').Skip(1).Where(s => s.Any()).Select(long.Parse).ToList();
        var distances = lines[1].Split(' ').Skip(1).Where(s => s.Any()).Select(long.Parse).ToList();

        var races = new List<Race>();
        for (var i = 0; i < times.Count; i++) races.Add(new Race(times[i], distances[i]));

        var part1 = races.Aggregate(1L, (acc, race) => acc * race.WaysToWin());
        Console.WriteLine($"Part 1: {part1}");

        var part2Race = new Race(long.Parse(string.Concat(lines[0].Where(char.IsDigit))),
            long.Parse(string.Concat(lines[1].Where(char.IsDigit))));
        Console.WriteLine($"Part 2: {part2Race.WaysToWin()}");
    }
}