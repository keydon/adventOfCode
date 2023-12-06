using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Digit {
        public int NumericValue { get; set;}
        public string AsNumberStr {get; set; }
        public string AsWord {get; set; }

        public Digit(int value, string word){
            NumericValue = value;
            AsWord = word;
            AsNumberStr = value.ToString();
        }
    }
    record Occurance {
        public int Index { get; set; }
        public Digit Digit { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var doc = LoadCalibrationDocument("input.txt");
            //doc = LoadCalibrationDocument("sample2.txt");

            var digits = new List<Digit>(){
                new (0, "zero"),
                new (1, "one"),
                new (2, "two"),
                new (3, "three"),
                new (4, "four"),
                new (5, "five"),
                new (6, "six"),
                new (7, "seven"),
                new (8, "eight"),
                new (9, "nine"),
            };

            SumOfRecoveredCalibrarionValues(doc, digits, FindNumericDigits)
                .AsResult1();
            SumOfRecoveredCalibrarionValues(doc, digits, FindNumericAndWordDigits)
                .AsResult2();

            Report.End();
        }

        private static string[] FindNumericDigits(Digit d) => new[] { d.AsNumberStr };

        private static string[] FindNumericAndWordDigits(Digit d) => new[] { d.AsNumberStr, d.AsWord };

        private static long SumOfRecoveredCalibrarionValues(List<string> doc, List<Digit> digits, Func<Digit,string[]> findingStrategy)
        {
            return doc
                .Select(line => digits
                    .SelectMany(digit => FindOccurances(digit, line, findingStrategy))
                    .OrderBy(occ => occ.Index)
                    .ToList()
                )
                .Select(occurances => CombineFirstAndLastDigit(occurances))
                .Sum();
        }

        private static IEnumerable<Occurance> FindOccurances(Digit digit, string haystack, Func<Digit,string[]> findingStrategy)
        {
            var needles = findingStrategy(digit);
            foreach (var needle in needles)
            {
                var first = new Occurance() {
                    Index = haystack.IndexOf(needle),
                    Digit = digit
                };
                var last = new Occurance(){
                    Index = haystack.LastIndexOf(needle),
                    Digit = digit
                };
                if (first.Index > -1) {
                    yield return first; 
                    yield return last;
                }
            };
        }

        private static long CombineFirstAndLastDigit(List<Occurance> occurances)
        {
            var first = occurances.First();
            var last = occurances.Last();
            var calibrationValue = long.Parse(
                first.Digit.AsNumberStr + last.Digit.AsNumberStr
            );

            return calibrationValue;
        }

        public static List<string> LoadCalibrationDocument(string inputTxt)
        {
            var lines = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return lines;
        }
    }
}
