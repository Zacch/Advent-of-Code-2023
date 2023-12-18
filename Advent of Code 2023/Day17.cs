namespace Advent_of_Code_2023;

public abstract class Day17
{
    private enum Direction { N, W, E, S }

    private class State : IEquatable<State>
    {
        public readonly int Row;
        public readonly int Col;
        public readonly int HeatLost;
        public readonly Direction Direction; // 0 = N, 1 = E, 2 = S, 3 = W
        public readonly int Straight;

        public State(int row, int col, int heatLost, Direction direction, int straight)
        { Row = row; Col = col; HeatLost = heatLost; Direction = direction; Straight = straight; }

        public bool Equals(State? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Row == other.Row && Col == other.Col;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((State) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Row, Col);

        public override string ToString()
        {
            return $"{nameof(Row)}: {Row}, {nameof(Col)}: {Col}, {nameof(HeatLost)}: {HeatLost}, " +
                   $"{nameof(Direction)}: {Direction}, {nameof(Straight)}: {Straight}";
        }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day17.txt").ToList();
        var grid = new List<List<int>>();
        foreach (var line in lines) grid.Add(line.Select(c => c - '0').ToList());
        var goalRow = grid.Count - 1;
        var goalCol = grid[0].Count - 1;

        var frontier = new PriorityQueue<State, int>();
        frontier.Enqueue(new State(0, 0, 0, Direction.S, 0), 0);
        var visited = new HashSet<State>();

        var done = false;
        var highestRow = 0;
        while (!done)
        {
            var current = frontier.Dequeue();
            foreach (var next in Neighbours(current, grid))
            {
                visited.TryGetValue(next, out var old);
                if (old != null && old.HeatLost <= next.HeatLost) continue;
                if (next.Row == goalRow && next.Col == goalCol)
                {
                    Console.WriteLine($"Part1: {next.HeatLost}");
                    done = true;
                    break;
                }

                var priority = next.HeatLost + (goalRow - next.Row) + (goalCol - next.Col);
                frontier.Enqueue(next, priority);
            }

            visited.Add(current);
            if (current.Row + current.Col > highestRow)
            {
                Console.WriteLine(current);
                highestRow = current.Row + current.Col;
            }
        }


        var part2 = 0;
        Console.WriteLine($"Part2: {part2}");
    }

    private static List<State> Neighbours(State state, List<List<int>> grid)
    {
        var neighbours = new List<State>();
        if (state.Direction != Direction.N && state.Row + 1 < grid.Count)
        {
            if (state.Direction != Direction.S || state.Straight < 3)
            {
                var heatloss = grid[state.Row + 1][state.Col];
                neighbours.Add(new State(state.Row + 1, state.Col, state.HeatLost + heatloss,
                    Direction.S, state.Direction == Direction.S ? state.Straight + 1 : 1));
            }
        }
        if (state.Direction != Direction.W && state.Col + 1 < grid[state.Row].Count)
        {
            if (state.Direction != Direction.E || state.Straight < 3)
            {
                var heatloss = grid[state.Row][state.Col + 1];
                neighbours.Add(new State(state.Row, state.Col + 1, state.HeatLost + heatloss,
                    Direction.E, state.Direction == Direction.E ? state.Straight + 1 : 1));
            }
        }
        if (state.Direction != Direction.E && state.Col > 0)
        {
            if (state.Direction != Direction.W || state.Straight < 3)
            {
                var heatloss = grid[state.Row][state.Col - 1];
                neighbours.Add(new State(state.Row, state.Col - 1, state.HeatLost + heatloss,
                    Direction.W, state.Direction == Direction.W ? state.Straight + 1 : 1));
            }
        }
        if (state.Direction != Direction.S && state.Row > 0)
        {
            if (state.Direction != Direction.N || state.Straight < 3)
            {
                var heatloss = grid[state.Row - 1][state.Col];
                neighbours.Add(new State(state.Row - 1, state.Col, state.HeatLost + heatloss,
                    Direction.N, state.Direction == Direction.N ? state.Straight + 1 : 1));
            }
        }

        return neighbours;
    }
}

// 1090 is too high