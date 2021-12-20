using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record InsertionRule
    {
        public string Needle { get; set; }
        public string Insert { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var (template, insertionRules) = LoadInput("input.txt");
            // (template, insertionRules) = LoadInput("sample.txt");
            
            Polimerize(insertionRules, template, 10).AsResult1();
            Polimerize(insertionRules, template, 40).AsResult2();

            Report.End();
        }

        private static long Polimerize(List<InsertionRule> rules, string template, int steps)
        {
            var newPairs = CountPairs(template);

            for (int i = 0; i < steps; i++)
            {
                var oldPairs = new Dictionary<string, long>(newPairs);
                foreach (var old in rules)
                {
                    var newLeft = old.Needle.First() + old.Insert;
                    var newRight = old.Insert + old.Needle.Last();

                    var count = oldPairs.TryGetDef(old.Needle, 0L);
                    newPairs.Change(old.Needle, (c) => c - count);
                    newPairs.Change(newLeft,  (c) => c + count);
                    newPairs.Change(newRight, (c) => c + count);
                }
            }

            var occurences = CountLetters(newPairs);

            var (_, leastCount) = occurences.First().Debug("least");
            var (_, mostCount) = occurences.Last().Debug("most");

            return mostCount - leastCount;
        }

        private static Dictionary<string, long> CountPairs(string template)
        {
            var newPairs = new Dictionary<string, long>
            {
                ["_" + template[0]] = 1L, // left edge
                [template[^1] + "_"] = 1L // right edge
            };
            for (int i = 0; i < template.Length - 1; i++)
            {
                var pair = $"{template[i]}{template[i + 1]}";
                var count = newPairs.TryGetDef(pair, 0L);
                newPairs[pair] = count + 1;
            }

            return newPairs;
        }

        private static List<(char letter, long Sum)> CountLetters(Dictionary<string, long> newPairs)
        {
            var occurences = newPairs
                .SelectMany(SplitPairCount)
                .Where(l => l.letter != '_')
                .GroupBy(key => key.letter, val => val.count, 
                    (letter, counts) => (letter, Sum: counts.Sum()/2))
                .OrderBy(l => l.Sum)
                .Peek()
                .ToList();
            return occurences;
        }

        private static IEnumerable<(char letter, long count)> SplitPairCount(KeyValuePair<string, long> pair)
        {
            return pair.Key.Select(c => (c, pair.Value));
        }

        public static (string template, List<InsertionRule> insertions) LoadInput(string inputTxt)
        {
            var parts = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .ToList();

            string template = parts.First().First();

            var insertions = parts.Last()
                .Select(s => s.ParseRegex(@"^(.+) -> (.+)$", 
                    m => new InsertionRule() {
                        Needle = m.Groups[1].Value,
                        Insert = m.Groups[2].Value,
                    }))
                .ToList();

            return (template, insertions);
        }
    }
}
