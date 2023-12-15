namespace Advent_of_Code_2023;

public abstract class Day15
{
    private class Lens : IEquatable<Lens>
    {
        public readonly string Label;
        public int FocalLength;
        public readonly int Box;

        public Lens(string label, int focalLength)
        {
            FocalLength = focalLength;
            Label = label;
            Box = Hash(label);
        }

        public override string ToString() =>
            $"[{nameof(Label)}: {Label}, {nameof(FocalLength)}: {FocalLength}, {nameof(Box)}: {Box}]";

        public bool Equals(Lens? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Label == other.Label;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Lens) obj);
        }

        public override int GetHashCode() => Label.GetHashCode();
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day15.txt").ToList();
        var part1 = 0;
        var boxes = new Dictionary<int, List<Lens>>();
        foreach (var step in lines[0].Split(','))
        {
            part1 += Hash(step);
            if (step.Contains('='))
            {
                var parts = step.Split('=');
                var lens = new Lens(parts[0], int.Parse(parts[1]));
                boxes.TryAdd(lens.Box, new List<Lens>());
                var contents = boxes[lens.Box];
                var i = contents.IndexOf(lens);
                if (i >= 0) contents[i].FocalLength = lens.FocalLength;
                else contents.Add(lens);
            }
            else
            {
                var lens = new Lens(step[..^1], 0);
                boxes.TryAdd(lens.Box, new List<Lens>());
                boxes[lens.Box].RemoveAll(l => l.Label == lens.Label);
            }
        }
        Console.WriteLine($"Part1: {part1}");

        var part2 = 0;
        foreach (var (boxNumber, lenses) in boxes)
            for (var i = 0; i < lenses.Count; i++) part2 += (boxNumber + 1) * (i + 1) * lenses[i].FocalLength;
        Console.WriteLine($"Part2: {part2}");
    }

    private static int Hash(string line) => line.ToCharArray().Aggregate(0, (i, c) => (i + c) * 17 % 256);
}