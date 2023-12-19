// ReSharper disable InconsistentNaming

using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public abstract class Day19
{
    private class Rule
    {
        public readonly char Property;
        public readonly char Operation;
        public readonly int Operand;
        public readonly string Destination;

        public bool MatchesAll => Operation == '-';

        public Rule(char property, char operation, int operand, string destination)
        { Property = property; Operation = operation; Operand = operand; Destination = destination; }
        public Rule(string destination) { Property = '-'; Operation = '-'; Operand = -1; Destination = destination; }

        public bool Matches(Part part)
        {
            if (MatchesAll) return true;
            var value = Property switch
            {
                'x' => part.x,
                'm' => part.m,
                'a' => part.a,
                's' => part.s,
                _ => throw new ApplicationException()
            };
            return Operation switch
            {
                '>' => value > Operand,
                '<' => value < Operand,
                _ => throw new ApplicationException()
            };
        }
    }

    private class Workflow
    {
        public string Name;
        public List<Rule> Rules;

        public readonly List<Part> Parts = new();

        public Workflow(string name, List<Rule> rules)
        { Name = name; Rules = rules; }
    }
    
    private class Part
    {
        public readonly int x;
        public readonly int m;
        public readonly int a;
        public readonly int s;
        public int RatingSum => x + m + a + s;

        public Part(int x, int m, int a, int s)
        { this.x = x; this.m = m; this.a = a; this.s = s; }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day19.txt").ToList();
        var workflows = new Dictionary<string, Workflow>();
        var parts = new List<Part>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith('{'))
            {
                var match = Regex.Match(line, "\\{x=(\\d+),m=(\\d+),a=(\\d+),s=(\\d+)\\}");
                parts.Add(new Part(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value)));
            }
            else
            {
                var lineParts = line[..^1].Split('{');

                var ruleStrings = lineParts[1].Split(',');
                var rules = new List<Rule>();
                foreach (var ruleString in ruleStrings)
                {
                    if (ruleString.Contains(':'))
                    {
                        var stringParts = ruleString.Split(':');
                        rules.Add(new Rule(stringParts[0][0], stringParts[0][1],
                            int.Parse(stringParts[0][2..]), stringParts[1]));
                    }
                    else
                    {
                        rules.Add(new Rule(ruleString));
                    }
                }

                workflows[lineParts[0]] = new Workflow(lineParts[0], rules);
            }
        }

        var part1 = parts.Where(part => IsAccepted(part, workflows)).Sum(part => part.RatingSum);
        Console.WriteLine($"Part1: {part1}");

        var part2 = 0;
        Console.WriteLine($"Part2: {part2}");
    }

    private static bool IsAccepted(Part part, Dictionary<string,Workflow> workflows)
    {
        var workflowName = "in";
        while (workflowName != "A")
        {
            Console.WriteLine(workflowName);
            var workflow = workflows[workflowName];
            foreach (var rule in workflow.Rules)
            {
                if (rule.Matches(part))
                {
                    workflowName = rule.Destination;
                    if (workflowName == "R") return false;
                    break;
                }
            }
        }

        return true;
    }
}