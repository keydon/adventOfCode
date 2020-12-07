using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace _07
{
    record Bag
    {
        public string Name { get; set; }
        public int Amount { get; set; }

        public List<Bag> InnerBags { get; set; } = new List<Bag>();
    }
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var foos = LoadBags("input.txt");

            var allFoos = foos.ToDictionary(f => f.Name);
            var memo = new Dictionary<string, int>();

            var result1 = CountBagsContainingShinyGold(allFoos, memo);
            Console.WriteLine($"Part1-Result: {result1}");

            memo.Clear();
            var result2 = CountTotalBags(allFoos, allFoos["shiny gold"], memo);
            Console.WriteLine($"Part2-Result: {result2}");
        }

        private static int CountBagsContainingShinyGold(Dictionary<string, Bag> foos, Dictionary<string, int> memo)
        {
            foreach (var foo in foos)
            {
                HasShinyGold(foos, foo.Value, memo);
            }
            return memo.Where(kvp => kvp.Value >= 1).Count();
        }
        private static int HasShinyGold(Dictionary<string, Bag> foos, Bag foo, Dictionary<string, int> memo)
        {
            if (memo.ContainsKey(foo.Name))
            {
                return memo[foo.Name];
            }
            if (foo.Name.Trim() == "shiny gold")
            {
                return 1;
            }

            var amount = 0;
            foreach (var innerBag in foo.InnerBags)
            {
                amount += HasShinyGold(foos, foos[innerBag.Name], memo);
            }
            memo[foo.Name] = amount;
            return amount;
        }

        private static int CountTotalBags(Dictionary<string, Bag> foos, Bag foo, Dictionary<string, int> memo)
        {
            if (memo.ContainsKey(foo.Name))
            {
                return memo[foo.Name];
            }

            var amount = 0;
            foreach (var innerBag in foo.InnerBags)
            {
                amount += innerBag.Amount + innerBag.Amount * CountTotalBags(foos, foos[innerBag.Name], memo);
            }
            memo[foo.Name] = amount;
            return amount;
        }


        public static List<Bag> LoadBags(string inputTxt, int top = 0)
        {
            var bags = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select((row) =>
                {
                    var parts = row.Splizz(" bags contain ", ", ");
                    var first = parts.First();
                    var other = parts.Skip(1).ToList();
                    return new Bag()
                    {
                        Name = first,
                        InnerBags = other.Select(inner => ParseRegex(inner)).Where(y => y != null).ToList()
                    };

                });

            var bagsList = bags.ToList();
            Console.WriteLine($"Loaded {bags.Count()} entries ({inputTxt})");
            return bagsList;
        }

        private static Bag ParseRegex(string line)
        {
            if (line == "no other bags.")
            {
                return null;
            }
            // 
            // clear maroon bags contain 1 dull lavender bag.
            // 
            Regex operationRegEx = new Regex(@"^(\d+) ([^,]+) (bag|bags)\.?$");
            var match = operationRegEx.Match(line.Trim());
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);

            return new Bag()
            {
                Amount = int.Parse(match.Groups[1].Value),
                Name = match.Groups[2].Value
            };
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
