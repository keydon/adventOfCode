using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc
{
    record Spoken
    {
        public int Number { get; set; }
        public int Position { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = LoadStartingNumbers("input.txt");
            GetNthSpokenNumber(numbers, 2020).AsResult1();
            GetNthSpokenNumber(numbers, 30000000).AsResult2();
            Report.End();
        }

        private static int GetNthSpokenNumber(IEnumerable<Spoken> foos, int theNthNumber)
        {
            var spoken = foos.SkipLast(1).ToDictionary(n => n.Number, n => n.Position);

            var lastSpoken = foos.Last().Number;
            var count = foos.Count();
            while (true)
            {
                var lastPosition = count;
                count++;

                if (spoken.TryGetValue(lastSpoken, out var lastBeforePosition))
                {
                    spoken[lastSpoken] = lastPosition;
                    var age = lastPosition - lastBeforePosition;
                    lastSpoken = age;
                }
                else
                {
                    spoken[lastSpoken] = lastPosition;
                    lastSpoken = 0;
                }
                if (count == theNthNumber)
                {
                    return lastSpoken;
                }
            }
        }

        public static IEnumerable<Spoken> LoadStartingNumbers(string inputTxt, int top = 0)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany(r => r.Splizz(",", ";")
                    .Select((x, i) => new Spoken() { Number = int.Parse(x), Position = i + 1 }));

            return foos;
        }
    }

    public static class StringExtensions
    {
        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
    }
}
