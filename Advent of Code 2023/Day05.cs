namespace Advent_of_Code_2023;

public abstract class Day05
{
    private class Mapping
    {
        public readonly long Start, Destination, Length;

        public long End => Start + Length - 1;
        public Mapping(long destination, long source, long length)
        { Length = length; Destination = destination; Start = source; }

        public bool Matches(long i) => i >= Start && i < Start + Length;
        public long Map(long i) => Matches(i) ? Destination + i - Start : i;
    }

    private class LongRange
    {
        public readonly long Start, End;
        public long Length => End + 1 - Start;
        public LongRange(long start, long length) { Start = start; End = start + length - 1; }
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
        for (var i = 0; i < seeds.Count - 1; i+= 2)
            seedRanges.Add(new LongRange(seeds[i], seeds[i + 1]));

        foreach (var mapList in maps)
        {
            var mappedRanges = new List<LongRange>();
            foreach (var range in seedRanges)
            {
                var rangesToMap = new List<LongRange>{range};
                foreach (var mapping in mapList)
                {
                    Map(rangesToMap, mapping, mappedRanges);
                    if (!rangesToMap.Any()) break;
                }
                mappedRanges.AddRange(rangesToMap);
            }
            seedRanges = mappedRanges;
        }

        Console.WriteLine($"Part 2: {seedRanges.Min(s => s.Start)}");
    }

    private static void Map(List<LongRange> rangesToMap, Mapping mapping, List<LongRange> mappedRanges)
    {
        var rangesStillToMap = new List<LongRange>();
        foreach (var range in rangesToMap)
            MapRange(range, mapping, rangesStillToMap, mappedRanges);
        rangesToMap.Clear();
        rangesToMap.AddRange(rangesStillToMap);
    }

    private static void MapRange(LongRange seedRange, Mapping mapping, List<LongRange> rangesStillToMap, List<LongRange> mappedRanges)
    {
        if (seedRange.End < mapping.Start || seedRange.Start > mapping.End)
        {
            rangesStillToMap.Add(seedRange);
            return;
        }

        if (seedRange.Start < mapping.Start)
        {
            var prefixLength = mapping.Start - seedRange.Start;
            rangesStillToMap.Add(new LongRange(seedRange.Start, prefixLength));
            mappedRanges.Add(new LongRange(mapping.Destination, Math.Min(mapping.Length, seedRange.Length - prefixLength)));
            if (seedRange.End > mapping.End)
                rangesStillToMap.Add(new LongRange(mapping.End + 1, seedRange.Length - prefixLength - mapping.Length));
        }
        else
        {
            mappedRanges.Add(new LongRange(mapping.Map(seedRange.Start),
                Math.Min(seedRange.Length, mapping.End - seedRange.Start + 1)));
            if (seedRange.End > mapping.End)
                rangesStillToMap.Add(new LongRange(mapping.End + 1, seedRange.End - mapping.End));
        }
    }
}