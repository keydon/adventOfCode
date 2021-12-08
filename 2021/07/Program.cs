using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var crabs = LoadCrabs("input.txt");

            var min = crabs.Min().Debug("Min");
            var max = crabs.Max().Debug("Max");

            var part1 = Enumerable
                .Range(min, max-min+1)
                .Select(target => (target, fuel: crabs.Select(from => Math.Abs(target - from)).Sum()))
                .OrderBy(p => p.fuel)
                .First();
            part1.target.Debug("Cheapest Position");
            part1.fuel.Debug("Total Fuel");
            part1.fuel.AsResult1();

            var part2 = Enumerable
                .Range(min-1, max-min+1)
                .Select(target => (target, fuel: crabs.Select(from => 
                        Enumerable.Range(0, Math.Abs(target - from) + 1)
                            .Aggregate(0, (total, step) => total + step))
                            .Sum()
            ))
            .OrderBy(p => p.fuel)
            .First();

            part2.target.Debug("Cheapest Position");
            part2.fuel.Debug("Total Fuel");
            part2.fuel.AsResult2();
            Report.End();
        }

        public static List<int> LoadCrabs(string inputTxt)
        {
            var crabs = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany(r => r.Splizz(",", ";"))
                .Select(int.Parse)
                .ToList();

            return crabs;
        }
    }
}
