using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace _09
{
    class Program
    {
        static void Main(string[] args)
        {
            var foos = LoadFoos("input.txt");

            var result1 = CalculatePart1(foos);
            Console.WriteLine($"Part1-Result: {result1}");

            var result2 = CalculatePart2(foos, result1);
            Console.WriteLine($"Part2-Result: {result2}");
        }

        private static long CalculatePart2(List<long> foos, long result1)
        {
            for (int i = 0; i < foos.Count; i++)
            {
                var take = 2;
                long sum = 0;
                while (sum < result1)
                {
                    var candidates = foos.Skip(i).Take(take);
                    sum = candidates.Sum();
                    if (sum == result1)
                    {
                        return candidates.Min() + candidates.Max();
                    }
                    take++;
                }
            }

            throw new Exception("Cannot find the solution for part II :/");
        }

        private static long CalculatePart1(List<long> foos)
        {
            var preamble = 25;
            for (int i = preamble; i < foos.Count; i++)
            {
                var candidate = foos[i];
                var possible = foos.Skip(i - preamble).Take(preamble).ToList();
                if (possible.Any(x => possible.Contains(candidate - x)))
                {
                    continue;
                }
                return candidate;
            }
            throw new Exception("Cannot find the solution for part I");
        }

        public static List<long> LoadFoos(string inputTxt, int top = 0)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(long.Parse);

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
        }
    }
}
