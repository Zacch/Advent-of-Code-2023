using Advent_of_Code_2023.Utilities;
// ReSharper disable UseObjectOrCollectionInitializer

namespace Advent_of_Code_2023;

public abstract class Day18
{
    private class Instruction
    {
        public static readonly Instruction None = new Instruction('\0', 0, "");
        public readonly char Direction;
        public readonly int Length;
        public readonly string Color;
        public Instruction Previous = None;
        public Instruction Next = None;

        public Instruction(char direction, int length, string color)
        { Direction = direction; Length = length; Color = color; }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day18.txt").ToList();
        var instructions = new List<Instruction>();

        var location = new Point(0, 0);
        var dug = new Dictionary<Point, Instruction>();
        dug[location] = new Instruction('U', 0, "#000000");
        var minx = 0; var miny = 0;
        var maxx = 0; var maxy = 0;
        var previous = Instruction.None;

        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            var instruction = new Instruction(parts[0].First(), int.Parse(parts[1]), parts[2][1..^1]);
            instruction.Previous = previous;
            if (previous.Direction != '\0') previous.Next = instruction;
            instructions.Add(instruction);
            previous = instruction;
            for (var i = 0; i < instruction.Length; i++)
            {
                switch (instruction.Direction)
                {
                    case 'U':
                        location = new Point(location.x, location.y - 1);
                        break;
                    case 'L':
                        location = new Point(location.x - 1, location.y);
                        break;
                    case 'R':
                        location = new Point(location.x + 1, location.y);
                        break;
                    case 'D':
                        location = new Point(location.x, location.y + 1);
                        break;
                    default:
                        throw new ApplicationException();
                }
                dug[location] = instruction;
                minx = Math.Min(minx, location.x);
                maxx = Math.Max(maxx, location.x);
                miny = Math.Min(miny, location.y);
                maxy = Math.Max(maxy, location.y);
                if (location.y == -343)
                {
                    Console.WriteLine($"{line} -- {location}");
                }
            }
        }


        var part1 = 0;
        for (var y = miny - 2; y <= maxy - 2; y++)
        {
            var row = new List<char>();
            var inside = false;
            var horizontalDig = false;
            var cameFromAbove = false;
            for (var x = minx - 2; x <= maxx + 2; x++)
            {
                var hole = inside;
                if (dug.TryGetValue(new Point(x, y), out var instruction))
                {
                    if (instruction.Direction is 'U' or 'D')
                    {
                        horizontalDig = false;
                        inside = !inside;
                        if (inside) hole = true;
                    }
                    else
                    {
                        if (!horizontalDig)
                        {
                            switch (instruction.Direction)
                            {
                                case 'R':
                                    if (instruction.Previous.Direction != 'D' && instruction.Previous.Direction != 'U')
                                        throw new ApplicationException();
                                    cameFromAbove = instruction.Previous.Direction == 'D';
                                    break;

                            }
                        }
                        horizontalDig = true;
                        inside = true;
                    }
                }
                else
                {
                    if (horizontalDig)
                    {
                        horizontalDig = false;
                        inside = false;
                    }
                }
                row.Add(hole ? '#' : '.');
                if (hole) part1++;
            }

            if (inside)
            {
                Console.WriteLine("");
            }
            Console.WriteLine(string.Join("", row));
        }
        Console.WriteLine($"Part1: {part1}");

        var part2 = 0;
        Console.WriteLine($"Part2: {part2}");
    }
}

// 73964 is too high
// 64012 is too low