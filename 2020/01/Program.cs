using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var expenses = LoadExpenseReport("input.txt");

            SolvePartOne(expenses);

            stopwatch.Stop();
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();

            SolvePartTwo(expenses);

            stopwatch.Stop();
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void SolvePartOne(List<int> expenses)
        {
            foreach (var a in expenses)
            {
                foreach (var b in expenses)
                {
                    if (a + b == 2020)
                    {
                        Console.WriteLine($"Answer is {a * b}");
                        return;
                    }
                }
            }
        }

        private static void SolvePartTwo(List<int> expenses)
        {
            foreach (var a in expenses)
            {
                foreach (var b in expenses)
                {
                    foreach (var c in expenses)
                    {
                        if (a + b + c == 2020)
                        {
                            Console.WriteLine($"Answer is {a * b * c}");
                            return;
                        }
                    }
                }
            }
        }

        public static List<int> LoadExpenseReport(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .Select(int.Parse)
                .ToList();
        }
    }
}
