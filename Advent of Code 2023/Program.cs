
using System.Diagnostics;
using Advent_of_Code_2023;
var timer = new Stopwatch();
timer.Start();

Day13.Solve();

timer.Stop();
Console.WriteLine($"Done in {timer.ElapsedMilliseconds/1000d} seconds");