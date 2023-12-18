using Advent_of_Code_2023.Utilities;
using static Advent_of_Code_2023.Day10;
// ReSharper disable AccessToModifiedClosure, UseObjectOrCollectionInitializer

namespace Advent_of_Code_2023;

public abstract class Day18
{
    private class Instruction
    {
        public static readonly Instruction None = new('U', 0, "");
        public readonly char Direction;
        public readonly int Length;
        public readonly string Color;
        public Instruction Previous = None;
        public Instruction Next = None;
        public bool IsHorizontal => Direction is 'R' or 'L';

        public Instruction(char direction, int length, string color)
        { Direction = direction; Length = length; Color = color; }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day18.txt").ToList();
        var instructions = new List<Instruction>();

        var location = Point.Origin;
        var dug = new Dictionary<Point, char>();
        var minx = 0;
        var miny = 0;
        var maxx = 0;
        var maxy = 0;
        var previous = Instruction.None;

        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            var instruction = new Instruction(parts[0].First(), int.Parse(parts[1]), parts[2][1..^1]);
            instruction.Previous = previous;
            if (previous.Direction != '\0') previous.Next = instruction;
            instructions.Add(instruction);
            var firstLocation = true;
            for (var i = 0; i < instruction.Length; i++)
            {
                char content;
                if (firstLocation)
                {
                    dug[location] = CornerLetter($"{previous.Direction}{instruction.Direction}");
                }

                switch (instruction.Direction)
                {
                    case 'U':
                        location = new Point(location.x, location.y - 1);
                        content = '|';
                        break;
                    case 'L':
                        location = new Point(location.x - 1, location.y);
                        content = '–';
                        break;
                    case 'R':
                        location = new Point(location.x + 1, location.y);
                        content = '–';
                        break;
                    case 'D':
                        location = new Point(location.x, location.y + 1);
                        content = '|';
                        break;
                    default:
                        throw new ApplicationException();
                }

                dug[location] = content;
                minx = Math.Min(minx, location.x);
                maxx = Math.Max(maxx, location.x);
                miny = Math.Min(miny, location.y);
                maxy = Math.Max(maxy, location.y);

                firstLocation = false;
            }

            previous = instruction;
        }

        var corner = $"{instructions.Last().Direction}{instructions.First().Direction}";
        dug[new Point(0, 0)] = CornerLetter(corner);

        var part1 = Area(new Point(minx, miny), new Point(maxx, maxy), dug);
        Console.WriteLine($"Part1: {part1}");

        Console.WriteLine($"Part2: {Part2(instructions)}");
    }

    private static char CornerLetter(string corner) =>
        corner switch
        {
            "UL" => '7',
            "UR" => 'F',
            "DL" => 'J',
            "DR" => 'L',
            "LU" => 'L',
            "LD" => 'F',
            "RU" => 'J',
            "RD" => '7',
            _ => throw new ApplicationException($"{nameof(corner)} is \"{corner}\"")
        };

    private static long Area(Point topLeft, Point bottomRight, Dictionary<Point, char> dug)
    {
        var area = 0L;
        for (var y = topLeft.y; y <= bottomRight.y; y++)
        {
            var inside = false;
            var incomingDirection = IncomingDirection.None;
            for (var x = topLeft.x; x <= bottomRight.x; x++)
            {
                var point = new Point(x, y);
                if (dug.ContainsKey(point))
                {
                    switch (dug[point])
                    {
                        case '|':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            inside = !inside;
                            break;
                        case 'L':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            incomingDirection = IncomingDirection.North;
                            break;
                        case 'F':
                            if (incomingDirection != IncomingDirection.None) throw new ApplicationException();
                            incomingDirection = IncomingDirection.South;
                            break;
                        case '–':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            break;
                        case 'J':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            if (incomingDirection == IncomingDirection.South) inside = !inside;
                            incomingDirection = IncomingDirection.None;
                            break;
                        case '7':
                            if (incomingDirection == IncomingDirection.None) throw new ApplicationException();
                            if (incomingDirection == IncomingDirection.North) inside = !inside;
                            incomingDirection = IncomingDirection.None;
                            break;
                        default:
                            throw new ApplicationException($"Argh: {dug[point]} {point}");
                    }

                    area++;
                    continue;
                }

                if (inside) area++;
            }
        }

        return area;
    }

    private class Line : IComparable<Line>
    {
        public readonly Instruction Instruction;
        public readonly Point TopLeft;
        public readonly Point BottomRight;

        public Line(Instruction instruction, Point topLeft, Point bottomRight)
        { Instruction = instruction; TopLeft = topLeft; BottomRight = bottomRight; }

        public int CompareTo(Line? other)
        {
            if (other == null) return -1;
            var result = TopLeft.x.CompareTo(other.TopLeft.x);
            if (result == 0)
            {
                if (Instruction.IsHorizontal && !other.Instruction.IsHorizontal) result = 1;
                if (!Instruction.IsHorizontal && other.Instruction.IsHorizontal) result = -1;
            }
            return result;
        }
    }

    private static long Part2(List<Instruction> instructions)
    {
        Console.WriteLine("\n(Please wait. This will take several minutes...)\n");
        var start = Point.Origin;
        var previous = Instruction.None;
        var lines = new List<Line>();

        foreach (var oldInstruction in instructions)
        {
            var direction = oldInstruction.Color.Last() switch
            {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                '3' => 'U',
                _ => throw new ApplicationException()
            };
            var instruction = new Instruction(direction, Convert.ToInt32(oldInstruction.Color[1..^1], 16), "");
            instruction.Previous = previous;
            if (previous.Direction != '\0') previous.Next = instruction;

            Point end;
            switch (instruction.Direction)
            {
                case 'U':
                    end = new Point(start.x, start.y - instruction.Length);
                    break;
                case 'L':
                    end = new Point(start.x - instruction.Length, start.y);
                    break;
                case 'R':
                    end = new Point(start.x + instruction.Length, start.y);
                    break;
                case 'D':
                    end = new Point(start.x, start.y + instruction.Length);
                    break;
                default:
                    throw new ApplicationException();
            }
            var topLeft = new Point(Math.Min(start.x, end.x), Math.Min(start.y, end.y));
            var bottomRight = new Point(Math.Max(start.x, end.x), Math.Max(start.y, end.y));

            lines.Add(new Line(instruction, topLeft, bottomRight));

            start = end;
            previous = instruction;
        }

        return Part2Area(lines);
    }

    private static long Part2Area(List<Line> lines)
    {
        var area = 0L;
        var topLeft = new Point(lines.Select(l => l.TopLeft.x).Min(), lines.Select(l => l.TopLeft.y).Min());
        var bottomRight =
            new Point(lines.Select(l => l.BottomRight.x).Max(), lines.Select(l => l.BottomRight.y).Max());
        var currentLines = new SortedSet<Line>();
        for (var y = topLeft.y; y <= bottomRight.y; y++)
        {
            currentLines.RemoveWhere(l => l.BottomRight.y < y);
            foreach (var line in lines.Where(l => l.TopLeft.y == y)) currentLines.Add(line);
            var inside = false;
            var previousX = topLeft.x;
            foreach (var line in currentLines)
            {
                if (inside)
                {
                    area += line.TopLeft.x - previousX;
                    if (line.Instruction.IsHorizontal)
                    {
                        area += line.Instruction.Length + 1;
                        if (line.Instruction.Previous.Direction == line.Instruction.Next.Direction) inside = !inside;
                    }
                    else
                    {
                        inside = false;
                        if (line.TopLeft.x > previousX && (y == line.TopLeft.y || y == line.BottomRight.y)) area--;
                    }
                }
                else
                {
                    if (line.Instruction.IsHorizontal)
                    {
                        area += line.Instruction.Length + 1;
                        if (line.Instruction.Previous.Direction == line.Instruction.Next.Direction) inside = !inside;
                    }
                    else
                    {
                        inside = true;
                        if (y > line.TopLeft.y && y < line.BottomRight.y) area++;
                    }
                }

                previousX = line.BottomRight.x;
            }
        }

        return area;
    }
}