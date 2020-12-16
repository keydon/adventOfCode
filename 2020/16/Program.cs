using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc
{
    record Range
    {
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsInRange(int number) => Start <= number && number <= End;
    }

    record Rule
    {
        public Range Range1 { get; set; }
        public Range Range2 { get; set; }
        public string Name { get; set; }
        public bool IsValidFor(int number) => Range1.IsInRange(number) || Range2.IsInRange(number);
        public bool FailsFor(int number) => !IsValidFor(number);
    }
    class Program
    {
        static void Main(string[] args)
        {
            var input = LoadInput("input.txt").ToList();
            var rules = ParseValidationRules(input);
            var myTicket = input[1].Skip(1).Select(t => t.ParseInts().ToList()).Single();
            var nearbyTickets = input[2].Skip(1).Select(t => t.ParseInts().ToList()).ToList();

            SumUpInvalidTicketNumbers(rules, nearbyTickets).AsResult1();

            var positionRules = DetermineRuleForPosition(rules, nearbyTickets);
            var departureRules = rules
                .Where(p => p.Name.StartsWith("departure"))
                .ToList();
            var departurePositions = positionRules
                .Where(kvp => departureRules.Contains(kvp.Value))
                .Select(kvp => kvp.Key);
            departurePositions
                .Select(pos => (long)myTicket[pos])
                .Aggregate((a, b) => a * b).AsResult2();

            Report.End();
        }

        private static Dictionary<int, Rule> DetermineRuleForPosition(List<Rule> rules, List<List<int>> nearbyTickets)
        {
            var positions = new Dictionary<int, List<Rule>>();
            var initializePositions = true;

            foreach (var ticket in nearbyTickets)
            {
                var isInvalidTicket = ticket.Any(n => !rules.Any(r => r.IsValidFor(n)));
                if (isInvalidTicket)
                    continue;

                if (initializePositions)
                {
                    for (int i = 0; i < ticket.Count; i++)
                    {
                        positions[i] = rules.ToList();
                    }
                    initializePositions = false;
                }
                foreach (var position in positions)
                {
                    position.GetRules().RemoveAll(
                        rule => rule.FailsFor(ticket[position.GetPosition()])
                    );
                }
            }

            ProcedureOfExclusion(positions);
            return positions.ToDictionary(
                p => p.GetPosition(),
                p => p.GetRules().Single()
            );
        }

        private static void ProcedureOfExclusion(Dictionary<int, List<Rule>> positions)
        {
            while (positions.HasAmbiguities())
            {
                var singleRules = positions
                    .Where(p => p.GetRules().Count == 1)
                    .Select(p => new
                    {
                        Pos = p.GetPosition(),
                        Rule = p.GetRules().Single()
                    })
                    .ToList();
                foreach (var position in positions)
                {
                    foreach (var singeRule in singleRules)
                    {
                        if (singeRule.Pos == position.GetPosition())
                            continue;
                        position.GetRules().Remove(singeRule.Rule);
                    }
                }
            }
        }

        private static int SumUpInvalidTicketNumbers(List<Rule> rules, List<List<int>> nearbyTickets)
        {
            return nearbyTickets
                .SelectMany(t => t)
                .Where(number => !rules.Exists(r => r.IsValidFor(number)))
                .Sum();
        }

        private static List<Rule> ParseValidationRules(List<List<string>> foos)
        {
            return foos[0].Select(s =>
            {
                var ruleParts = s.Splizz(": ", " or ");
                var ruleName = ruleParts.First();
                var ranges = ruleParts.Skip(1).Select(s => ParseRegex(s)).ToList();

                return new Rule()
                {
                    Name = ruleName,
                    Range1 = ranges[0],
                    Range2 = ranges[1]
                };
            }).ToList();
        }

        public static IEnumerable<List<string>> LoadInput(string inputTxt, int top = 0)
        {
            return File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator(string.Empty);
        }

        private static Range ParseRegex(string line)
        {
            Regex operationRegEx = new Regex(@"^(\d+)-(\d+)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);
            return new Range()
            {
                Start = int.Parse(match.Groups[1].Value),
                End = int.Parse(match.Groups[2].Value)
            };
        }
    }

    internal static class ReadabilityExtensions
    {
        internal static List<Rule> GetRules(this KeyValuePair<int, List<Rule>> entry)
        {
            return entry.Value;
        }
        internal static int GetPosition(this KeyValuePair<int, List<Rule>> entry)
        {
            return entry.Key;
        }
        internal static bool HasAmbiguities<T>(this Dictionary<int, List<T>> dic)
        {
            return dic.Any(p => p.Value.Count > 1);
        }
    }
}
