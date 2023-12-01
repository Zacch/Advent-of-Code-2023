namespace Advent_of_Code_2023;

public abstract class Day01
{
    private static readonly string[] Numbers = {"😎", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};

    public static void Solve()
    {
        var lines = File.ReadLines("Day01.txt").ToList();
        var part1 = 0;
        foreach (var line in lines)
        {
            var digits = line.Where(char.IsDigit).ToList();
            part1 += int.Parse($"{digits.First()}{digits.Last()}");
        }
        Console.WriteLine($"Part1: {part1}");

        var part2 = 0;
        foreach (var line in lines)
        {
            var digits = new List<int>();
            for (var i = 0; i < line.Length; i++)
            {
                if (char.IsDigit(line[i])) digits.Add(line[i] - '0');
                for (var j = 1; j < 10; j++)
                    if (line[i..].StartsWith(Numbers[j])) digits.Add(j);
            }
            part2 += int.Parse($"{digits.First()}{digits.Last()}");
        }
        Console.WriteLine($"Part2: {part2}");
    }
}