using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace _01
{
    record IndexedJoltage
    {
        public int Jolts { get; set; }
        public int Index { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var foos = LoadJoltageRatings("input.txt");

            foos.Add(0); // charging outlet
            foos.Add(foos.Max() + 3); // built-in device
            foos.Sort();

            var stepCount = new Dictionary<int, int>();
            var sorted = foos.Aggregate(0, (a, b) =>
            {
                var step = b - a;
                stepCount[step] = stepCount.GetValueOrDefault(step, 0) + 1;
                return b;
            });

            var result1 = stepCount[1] * stepCount[3];
            Console.WriteLine($"Part1-Result: {result1}");

            var memo = new Dictionary<IndexedJoltage, long>();
            var indexedJoltages = foos.Select((j, i) => new IndexedJoltage() { Jolts = j, Index = i }).ToList();

            memo[indexedJoltages.Last()] = 1;
            long possis = CalcPossibilities(indexedJoltages, indexedJoltages.First(), memo);
            Console.WriteLine($"Part2-Result: {possis}");
        }

        public static long CalcPossibilities(List<IndexedJoltage> ratings, IndexedJoltage current, Dictionary<IndexedJoltage, long> memo)
        {
            if (memo.ContainsKey(current))
            {
                return memo[current];
            }
            var res = ratings.Skip(current.Index + 1)
                .Take(3)
                .Where((next) => (next.Jolts - current.Jolts) <= 3)
                .Select(x => CalcPossibilities(ratings, x, memo))
                .Sum();
            memo[current] = res;
            return res;
        }

        public static List<int> LoadJoltageRatings(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(int.Parse);

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
        }
    }
}
