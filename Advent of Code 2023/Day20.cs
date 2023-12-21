namespace Advent_of_Code_2023;

public abstract class Day20
{
    private class Module
    {
        public char Type;
        public readonly string Name;
        public readonly List<Connection> Inputs = new();
        public readonly List<Connection> Outputs = new();

        // For flip-flops
        public bool On;

        // For conjunctions
        public Dictionary<string, bool> Memory = new();

        public Module(char type, string name) { Type = type; Name = name; }
    }

    private class Connection
    {
        public readonly Module Source;
        public readonly Module Destination;

        public Connection(Module source, Module destination) { Source = source; Destination = destination; }
    }

    private class History
    {
        public readonly int Time;
        public readonly long LowPulses;
        public readonly long HighPulses;

        public History(int time, long lowPulses, long highPulses)
        { Time = time; LowPulses = lowPulses; HighPulses = highPulses; }
    }

    public static void Solve()
    {
        var lines = File.ReadLines("Day20.txt").ToList();

        var button = new Module('a', "button");
        var broadcaster = new Module('b', "broadcaster");
        var buttonToBroadcaster = new Connection(button, broadcaster);
        button.Outputs.Add(buttonToBroadcaster);
        broadcaster.Inputs.Add(buttonToBroadcaster);

        var modules = new Dictionary<string, Module> { [broadcaster.Name] = broadcaster };
        var flipFlops = new List<Module>();

        foreach (var line in lines)
        {
            var parts = line.Split(" -> ");
            var type = parts[0][0];
            var name = parts[0][1..];
            var module = broadcaster;
            if (parts[0] != "broadcaster")
            {
                if (!modules.ContainsKey(name)) modules[name] = new Module(type, name);
                module = modules[name];
                if (module.Type == '?') module.Type = type;
                if (type == '%') flipFlops.Add(module);
            }

            foreach (var destinationName in parts[1].Split(", "))
            {
                if (!modules.ContainsKey(destinationName)) modules[destinationName] = new Module('?', destinationName);
                var destination = modules[destinationName];
                var connection = new Connection(module, destination);
                module.Outputs.Add(connection);
                destination.Inputs.Add(connection);
                destination.Memory[name] = false;
            }
        }

        var pulses = new Queue<(Connection, bool)>();
        var lowPulses = 0L;
        var highPulses = 0L;

        for (var i = 1uL; i < ulong.MaxValue; i++)
        {
            if (i % 1000000 == 0) Console.WriteLine($"Time {i / 1000000}m");
            // Console.WriteLine("");
            // Console.WriteLine($"Time {i}");
            pulses.Enqueue((buttonToBroadcaster, false));
            try
            {
                var tuple = RunCycle(pulses);
                highPulses += tuple.Item1;
                lowPulses += tuple.Item2;

                if (i == 1000) Console.WriteLine($"Part1: {highPulses * lowPulses} (High {highPulses}, Low {lowPulses})");
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Part2: {i} (High {highPulses}, Low {lowPulses})");
                throw;
            }
        }
    }

    private static (long, long) RunCycle(Queue<(Connection, bool)> pulses)
    {
        var lowPulses = 0L;
        var highPulses = 0L;

        var rxLowPulses = 0L;

        while (pulses.Count > 0)
        {
            var (connection, isHigh) = pulses.Dequeue();
            if (isHigh) highPulses++;
            else lowPulses++;

            // Console.WriteLine($"{connection.Source.Name} -{(isHigh ? "high" : "low")}-> {connection.Destination.Name}");
            var module = connection.Destination;
            if (!isHigh && module.Name == "rx") rxLowPulses++;
            switch (module.Type)
            {
                case 'b':
                    foreach (var output in module.Outputs) pulses.Enqueue((output, isHigh));
                    break;
                case '%':
                    if (isHigh) break;
                    module.On = !module.On;
                    foreach (var output in module.Outputs) pulses.Enqueue((output, module.On));
                    break;
                case '&':
                    module.Memory[connection.Source.Name] = isHigh;
                    var state = !module.Memory.Values.All(v => v);
                    foreach (var output in module.Outputs) pulses.Enqueue((output, state));
                    break;
            }
        }

        if (rxLowPulses == 1)
        {
            throw new ApplicationException("Good");
        }
        return (highPulses, lowPulses);
    }
}

// 11948078 is too low