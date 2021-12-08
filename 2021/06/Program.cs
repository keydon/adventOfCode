using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var fish = LoadLaternfish("input.txt");
            //foos = LoadFoos("sample.txt");

            CalculatePartI(fish, 80).AsResult1();

            CalculatePartII(fish, 256).AsResult2();

            Report.End();
        }

        private static int CalculatePartI(List<int> originalPopulation, int days)
        {
            var population = new List<int>(originalPopulation);
            for (int i = 0; i < days; i++)
            {
                var newGen = population.Where(f => f == 0).SelectMany(f => new int[] { 6, 8 }).ToList();
                var reduced = population.Where(f => f > 0).Select(f => f - 1).ToList();
                reduced.AddRange(newGen);
                population = reduced;

            }
            return population.Count;
        }

        private static long CalculatePartII(List<int> fish, int days)
        {
            var g2 = fish.GroupBy(f => f, f => f, (k, l) => (k, l.Count())).ToDictionary(p => p.k, p => (long)p.Item2); 

            for (int i = 0; i < days; i++)
            {
                var g1 = g2;
                g2 = new Dictionary<int, long>();
                
                for (int t = 0; t < g1.Keys.Max(); t++)
                {
                    if (g1.TryGetValue(t + 1, out long c))
                    {
                        g2[t] = c;
                    }
                }

                g2[6] = g2.TryGetDef(6, 0L) + g1.TryGetDef(0, 0L);
                g2[8] = g2.TryGetDef(8, 0L) + g1.TryGetDef(0, 0L);
            }

            return g2.Values.Sum();
        }

        public static List<int> LoadLaternfish(string inputTxt)
        {
            var fish = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany(r => r.Splizz(",", ";"))
                .Select(int.Parse)
                .ToList();

            return fish;
        }
    }
}
