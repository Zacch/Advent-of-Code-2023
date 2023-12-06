using System.Diagnostics;

namespace Advent_of_Code_2023;

public abstract class Day05
{
    private class Mapping
    {
        public readonly long Source, Destination, Length;

        public long Start => Source;
        public long End => Source + Length - 1;
        public Mapping(long destination, long source, long length)
        {
            Length = length;
            Destination = destination;
            Source = source;
        }

        public bool Matches(long i) => i >= Source && i < Source + Length;

        public long Map(long i)
        {
            return Matches(i) ? Destination + i - Source : i;
        }
    }

    private class LongRange
    {
        public readonly long Start, End;
        public long Length => End + 1 - Start;
        public LongRange(long start, long length) { Start = start; End = start + length - 1;}
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day05.txt").ToArray();

        var seeds = lines[0].Split(" ").Skip(1).Select(long.Parse).ToList();

        var maps = new List<List<Mapping>>();

        var currentList = new List<Mapping>();
        foreach (var line in lines.Skip(3))
        {
            if (line.Any() && char.IsDigit(line[0]))
            {
                var numbers = line.Split(" ").Select(long.Parse).ToArray();
                currentList.Add(new Mapping(numbers[0], numbers[1], numbers[2]));
            }
            else
            {
                if (currentList.Any())
                {
                    maps.Add(currentList);
                    currentList = new List<Mapping>();
                }
            }
        }

        if (currentList.Any()) maps.Add(currentList);

        TestMapRange();
        Part1(seeds, maps);
        Part2(seeds, maps);
    }

    private static void Part1(List<long> seeds, List<List<Mapping>> maps)
    {
        var mappedSeeds = new List<long>();

        foreach (var seed in seeds)
        {
            var mappedSeed = seed;
            foreach (var mapList in maps)
            {
                foreach (var mapping in mapList)
                {
                    if (mapping.Matches(mappedSeed))
                    {
                        mappedSeed = mapping.Map(mappedSeed);
                        break;
                    }
                }
            }

            mappedSeeds.Add(mappedSeed);
        }

        var part1 = mappedSeeds.Min();
        Console.WriteLine($"Part 1: {part1}");
    }

    private static void Part2(List<long> seeds, List<List<Mapping>> maps)
    {
        var seedRanges = new List<LongRange>();
        for (var i = 0; i < seeds.Count - 1; i+= 2) seedRanges.Add(new LongRange(seeds[i], seeds[i + 1]));

        var lists = 0;
        foreach (var mapList in maps)
        {
            foreach (var mapping in mapList)
            {
                var newRanges = new List<LongRange>();
                foreach (var seedRange in seedRanges)
                {
                    MapRange(seedRange, mapping, newRanges);
                }

                seedRanges = newRanges;
            }
            Console.WriteLine($"{++lists} map lists processed. {seedRanges.Count} seed ranges");
        }

        Console.WriteLine($"Part 2: {seedRanges.Min(s => s.Start)}");
    }

    private static void TestMapRange()
    {
        var mapping = new Mapping(50, 10, 10);
        var newRanges = new List<LongRange>();

        MapRange(new LongRange(5, 3), mapping, newRanges);
        if (newRanges.Count != 1) throw new ApplicationException();
        if (newRanges[0].Start != 5) throw new ApplicationException();
        if (newRanges[0].End != 7) throw new ApplicationException();

        newRanges.Clear();
        MapRange(new LongRange(15, 3), mapping, newRanges);
        if (newRanges.Count != 1) throw new ApplicationException();
        if (newRanges[0].Start != 55) throw new ApplicationException();
        if (newRanges[0].End != 57) throw new ApplicationException();

        newRanges.Clear();
        MapRange(new LongRange(5, 6), mapping, newRanges);
        if (newRanges.Count != 2) throw new ApplicationException();
        if (newRanges[0].Start != 5) throw new ApplicationException();
        if (newRanges[0].End != 9) throw new ApplicationException();
        if (newRanges[1].Start != 50) throw new ApplicationException();
        if (newRanges[1].End != 50) throw new ApplicationException();

        newRanges.Clear();
        MapRange(new LongRange(5, 15), mapping, newRanges);
        if (newRanges.Count != 2) throw new ApplicationException();
        if (newRanges[0].Start != 5) throw new ApplicationException();
        if (newRanges[0].End != 9) throw new ApplicationException();
        if (newRanges[1].Start != 50) throw new ApplicationException();
        if (newRanges[1].End != 59) throw new ApplicationException();

        newRanges.Clear();
        MapRange(new LongRange(5, 20), mapping, newRanges);
        if (newRanges.Count != 3) throw new ApplicationException();
        if (newRanges[0].Start != 5) throw new ApplicationException();
        if (newRanges[0].End != 9) throw new ApplicationException();
        if (newRanges[1].Start != 50) throw new ApplicationException();
        if (newRanges[1].End != 59) throw new ApplicationException();
        if (newRanges[2].Start != 20) throw new ApplicationException();
        if (newRanges[2].End != 24) throw new ApplicationException();

        newRanges.Clear();
        MapRange(new LongRange(30, 10), mapping, newRanges);
        if (newRanges.Count != 1) throw new ApplicationException();
        if (newRanges[0].Start != 30) throw new ApplicationException();
        if (newRanges[0].End != 39) throw new ApplicationException();


        newRanges.Clear();
        MapRange(new LongRange(19, 10), mapping, newRanges);
        if (newRanges.Count != 2) throw new ApplicationException();
        if (newRanges[0].Start != 59) throw new ApplicationException();
        if (newRanges[0].End != 59) throw new ApplicationException();
        if (newRanges[1].Start != 20) throw new ApplicationException();
        if (newRanges[1].End != 28) throw new ApplicationException();


        newRanges.Clear();
        MapRange(new LongRange(15, 10), mapping, newRanges);
        if (newRanges.Count != 2) throw new ApplicationException();
        if (newRanges[0].Start != 55) throw new ApplicationException();
        if (newRanges[0].End != 59) throw new ApplicationException();
        if (newRanges[1].Start != 20) throw new ApplicationException();
        if (newRanges[1].End != 24) throw new ApplicationException();
    }

    private static void MapRange(LongRange seedRange, Mapping mapping, List<LongRange> newRanges)
    {
        if (seedRange.End < mapping.Start || seedRange.Start > mapping.End)
        {
            newRanges.Add(seedRange);
            return;
        }

        if (seedRange.Start < mapping.Start)
        {
            var prefixLength = mapping.Start - seedRange.Start;
            newRanges.Add(new LongRange(seedRange.Start, prefixLength));
            newRanges.Add(new LongRange(mapping.Destination, Math.Min(mapping.Length, seedRange.Length - prefixLength)));
            if (seedRange.End > mapping.End)
                newRanges.Add(new LongRange(mapping.End + 1, seedRange.Length - prefixLength - mapping.Length));
        }
        else
        {
            newRanges.Add(new LongRange(mapping.Map(seedRange.Start),
                Math.Min(seedRange.Length, mapping.End - seedRange.Start + 1)));
            if (seedRange.End > mapping.End) newRanges.Add(new LongRange(mapping.End + 1, seedRange.End - mapping.End));
        }
    }
}

// 41872936 is too high