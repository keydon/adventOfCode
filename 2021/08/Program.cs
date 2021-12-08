using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
        internal class NotesEntry
    {
        public NotesEntry() { }

        public IEnumerable<string> Input { get; internal set; }
        public IEnumerable<string> Output { get; internal set; }

    }
    class Program
    {
        private static Dictionary<string, int> allDigits = new Dictionary<string, int>(){
                {"abcefg", 0},
                {"cf", 1},
                {"acdeg", 2},
                {"acdfg", 3},
                {"bcdf", 4},
                {"abdfg", 5},
                {"abdefg", 6},
                {"acf", 7},
                {"abcdefg", 8},
                {"abcdfg", 9},
            };
        private static Dictionary<string, int> uniquelySized = new Dictionary<string, int>(){
                {"cf", 1},
                {"acf", 7},
                {"bcdf", 4},
                {"abcdefg", 8}
            };

        static void Main(string[] args)
        {
            Report.Start();
            var entries = LoadNotes("input.txt");

            HashSet<int> uniqueLookup = uniquelySized.Keys.Select(s => s.Length).ToHashSet();
            entries
                .Select(entry => entry.Output.Count(encodedDigit => uniqueLookup.Contains(encodedDigit.Length)))
                .Sum().AsResult1();

            entries
                .Select(f => Decode(f))
                .Sum().AsResult2();

            Report.End();
        }

        private static int Decode(NotesEntry f)
        {
             var fivers = new Dictionary<string, int>(){
                {"acdeg", 2},
                {"acdfg", 3},
                {"abdfg", 5},
            };
            var sixers = new Dictionary<string, int>(){
                {"abcefg", 0},
                {"abdefg", 6},
                {"abcdfg", 9},
            };
            
            var known = new Dictionary<string, int>();
            var allWords = f.Input.Union(f.Output).ToList();

            var one = allWords.Where(w => w.Length == 2).Single();
            known.Add(one, 1);
            var seven = allWords.Where(w => w.Length == 3).Single();
            known.Add(seven, 7);
            var four = allWords.Where(w => w.Length == 4).Single();
            known.Add(four, 4);
            var eight = allWords.Where(w => w.Length == 7).Single();
            known.Add(eight, 8);

            var remainingWOrds = allWords.Except(new List<string>(){ one, four, seven, eight});

            var sixDigitWords = remainingWOrds.Where(w => w.Length == 6).ToList();
            var nine = sixDigitWords.Where(w => four.ToCharArray().All(c => w.Contains(c))).Single();
            known.Add(nine, 9);
            sixDigitWords.Remove(nine);
            var zero = sixDigitWords.Where(w => one.ToCharArray().All(c => w.Contains(c))).Single();
            known.Add(zero, 0);
            sixDigitWords.Remove(zero);
            var six = sixDigitWords.Single();
            known.Add(six, 6);
            
            var fiveDigitWords = remainingWOrds.Where(w => w.Length == 5).ToList();
            var two = fiveDigitWords.Where(w => !w.ToCharArray().All(c => nine.Contains(c))).Single();
            known.Add(two, 2);
            fiveDigitWords.Remove(two);
            var three = fiveDigitWords.Where(w => one.ToCharArray().All(c => w.Contains(c))).Single();
            known.Add(three, 3);
            fiveDigitWords.Remove(three);
            var five = fiveDigitWords.Single();
            known.Add(five, 5);

            return int.Parse(f.Output.Select(f => known[f]).ToCommaString(""));
        }

        public static List<NotesEntry> LoadNotes(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(l => {
                    var all = l.Splizz("|");
                    var input = all.First().Splizz(" ").Select(s => s.ToCharArray().OrderBy(c => c).ToCommaString("")).OrderBy(s => s);
                    var output = all.Last().Splizz(" ").Select(s => s.ToCharArray().OrderBy(c => c).ToCommaString(""));

                    return new NotesEntry() {
                    Input = input,
                    Output = output
                };
                })
             .ToList();

            return foos;
        }
    }
}
