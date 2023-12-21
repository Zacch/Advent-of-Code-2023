// ReSharper disable InconsistentNaming

using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public abstract class Day19
{
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

    private class Rule
    {
        public readonly char Property;
        public readonly char Operation;
        public readonly ulong Operand;
        public readonly string Destination;

        public bool MatchesAll => Operation == '-';

        public Rule(char property, char operation, ulong operand, string destination)
        { Property = property; Operation = operation; Operand = operand; Destination = destination; }
        public Rule(string destination) { Property = '-'; Operation = '-'; Operand = 0; Destination = destination; }

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
                '>' => (ulong) value > Operand,
                '<' => (ulong) value < Operand,
                _ => throw new ApplicationException()
            };
        }
    }

    private class Workflow
    {
        public readonly List<Rule> Rules;
        public Workflow(List<Rule> rules) => Rules = rules;
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
                            ulong.Parse(stringParts[0][2..]), stringParts[1]));
                    }
                    else
                    {
                        rules.Add(new Rule(ruleString));
                    }
                }

                workflows[lineParts[0]] = new Workflow(rules);
            }
        }

        Console.WriteLine($"Part1: {parts.Where(part => IsAccepted(part, workflows)).Sum(part => part.RatingSum)}");
        Console.WriteLine($"Part2: {Part2(workflows)}");
    }

    private static bool IsAccepted(Part part, Dictionary<string,Workflow> workflows)
    {
        var workflowName = "in";
        while (true)
        {
            var workflow = workflows[workflowName];
            var rule = workflow.Rules.First(rule => rule.Matches(part));
            workflowName = rule.Destination;
            switch (workflowName)
            {
                case "A": return true;
                case "R": return false;
            }
        }
    }

    private class ConstrainedPart : ICloneable
    {
        public ulong minx = 1;
        public ulong maxx = 4000;
        public ulong minm = 1;
        public ulong maxm = 4000;
        public ulong mina = 1;
        public ulong maxa = 4000;
        public ulong mins = 1;
        public ulong maxs = 4000;
        public string NextWorkflowName = "in";
        public ulong Combinations => (maxx - minx + 1) * (maxm - minm + 1) * (maxa - mina + 1) * (maxs - mins + 1);

        public object Clone() => MemberwiseClone();
    }

    private static ulong Part2(Dictionary<string, Workflow> workflows)
    {
        var frontier = new Queue<ConstrainedPart>();
        frontier.Enqueue(new ConstrainedPart());
        var matchingParts = new List<ConstrainedPart>();

        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            if (current.NextWorkflowName == "A")
            {
                matchingParts.Add(current);
                continue;
            }
            if (current.NextWorkflowName == "R") continue;
            var workflow = workflows[current.NextWorkflowName];
            foreach (var rule in workflow.Rules)
            {
                if (current.maxx == 0) break;
                if (rule.MatchesAll)
                {
                    if (rule.Destination == "A") matchingParts.Add(current);
                    else if (rule.Destination != "R")
                    {
                        current.NextWorkflowName = rule.Destination;
                        frontier.Enqueue(current);
                    }
                    break;
                }
                // Split the part in two
                var clone = (ConstrainedPart) current.Clone();
                clone.NextWorkflowName = rule.Destination;
                switch (rule.Property)
                {
                    case 'x':
                        switch (rule.Operation)
                        {
                            case '<':
                                if (current.minx >= rule.Operand) continue;
                                if (current.maxx < rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.maxx = rule.Operand - 1;
                                frontier.Enqueue(clone);
                                current.minx = rule.Operand;
                                break;
                            case '>':
                                if (current.maxx <= rule.Operand) continue;
                                if (current.minx > rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.minx = rule.Operand + 1;
                                frontier.Enqueue(clone);
                                current.maxx = rule.Operand;
                                break;
                        }
                        break;
                    case 'm':
                        switch (rule.Operation)
                        {
                            case '<':
                                if (current.minm >= rule.Operand) continue;
                                if (current.maxm < rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.maxm = rule.Operand - 1;
                                frontier.Enqueue(clone);
                                current.minm = rule.Operand;
                                break;
                            case '>':
                                if (current.maxm <= rule.Operand) continue;
                                if (current.minm > rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.minm = rule.Operand + 1;
                                frontier.Enqueue(clone);
                                current.maxm = rule.Operand;
                                break;
                        }
                        break;
                    case 'a':
                        switch (rule.Operation)
                        {
                            case '<':
                                if (current.mina >= rule.Operand) continue;
                                if (current.maxa < rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.maxa = rule.Operand - 1;
                                frontier.Enqueue(clone);
                                current.mina = rule.Operand;
                                break;
                            case '>':
                                if (current.maxa <= rule.Operand) continue;
                                if (current.mina > rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.mina = rule.Operand + 1;
                                frontier.Enqueue(clone);
                                current.maxa = rule.Operand;
                                break;
                        }
                        break;
                    case 's':
                        switch (rule.Operation)
                        {
                            case '<':
                                if (current.mins >= rule.Operand) continue;
                                if (current.maxs < rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.maxs = rule.Operand - 1;
                                frontier.Enqueue(clone);
                                current.mins = rule.Operand;
                                break;
                            case '>':
                                if (current.maxs <= rule.Operand) continue;
                                if (current.mins > rule.Operand)
                                {
                                    frontier.Enqueue(clone);
                                    current.maxx = 0;
                                    continue;
                                }

                                clone.mins = rule.Operand + 1;
                                frontier.Enqueue(clone);
                                current.maxs = rule.Operand;
                                break;
                        }
                        break;
                    default:
                        throw new ApplicationException();
                }
            }
        }

        ulong result = 0;
        foreach (var part in matchingParts)
        {
            result += part.Combinations;
        }
        return result;
    }
}