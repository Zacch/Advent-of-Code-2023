namespace Advent_of_Code_2023;

public abstract class Day02
{
    private class Round
    {
        public int Red;
        public int Green;
        public int Blue;
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day02.txt").ToList();

        var games = new Dictionary<int, List<Round>>();
        foreach (var line in lines)
        {
            var mainParts = line.Split(":");
            var gameNumber = int.Parse(mainParts[0].Split(" ")[1]);
            var game = new List<Round>();
            games[gameNumber] = game;
            foreach (var roundString in mainParts[1].Split(";"))
            {
                var round = new Round();
                var words = roundString.Replace(",", "").Trim().Split(" ");
                for (var i = 0; i + 1 < words.Length; i += 2)
                {
                    switch (words[i + 1])
                    {
                        case "red":
                            round.Red = int.Parse(words[i]);
                            break;
                        case "green":
                            round.Green = int.Parse(words[i]);
                            break;
                        case "blue":
                            round.Blue = int.Parse(words[i]);
                            break;
                    }
                }
                game.Add(round);
            }
        }

        var part1 = 0;
        foreach (var (number, game) in games)
            if (game.All(r => r is {Red: <= 12, Green: <= 13, Blue: <= 14})) part1 += number;
        Console.WriteLine($"Part 1: {part1}");

        var part2 = 0;
        foreach (var game in games.Values)
            part2 += game.Max(r => r.Red) * game.Max(r => r.Green) * game.Max(r => r.Blue);
        Console.WriteLine($"Part 2: {part2}");
    }
}