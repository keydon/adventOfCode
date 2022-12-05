using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Rearrangement
    {
        public int Amount { get; set; }
        public int From { get; set; }

        public int Dest { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var inputParts = LoadInputParts("input.txt");
            //inputParts = LoadInputParts("sample.txt");

            SolvePart1(inputParts);
            SolvePart2(inputParts);

            Report.End();
        }

        private static void SolvePart1(List<List<string>> inputParts)
        {
            var stacks = LoadStartingStacks(inputParts[0]);
            var rearrangements = LoadRearrangements(inputParts[1]);

            foreach (var move in rearrangements)
            {
                var fromStack = stacks[move.From - 1];
                var destStack = stacks[move.Dest - 1];
                foreach (var _ in Enumerable.Range(1, move.Amount))
                {
                    destStack.Push(fromStack.Pop());
                }
            }
            
            stacks.Select(s => s.Pop()).ToCommaString("").AsResult1();
        }

        
        private static void SolvePart2(List<List<string>> inputParts)
        {
            var stacks = LoadStartingStacks(inputParts[0]);
            var rearrangements = LoadRearrangements(inputParts[1]);

            foreach (var move in rearrangements)
            {
                var fromStack = stacks[move.From - 1];
                var destStack = stacks[move.Dest - 1];
                var tmp = new Stack<string>();
                foreach (var _ in Enumerable.Range(1, move.Amount))
                {
                    tmp.Push(fromStack.Pop());
                }
                foreach (var item in tmp)
                {
                    destStack.Push(item);
                }
            }

            stacks.Select(s => s.Pop()).ToCommaString("").AsResult2();
        }
        private static List<Stack<string>> LoadStartingStacks(List<string> stackDefinitions)
        {
            var reversedDefs = stackDefinitions.Select(x => x).Reverse().ToList();
            var stacks = reversedDefs
                .First()
                .Splizz(" ")
                .Select(_ => new Stack<string>())
                .ToList();

            foreach (var def in reversedDefs.Skip(1))
            {
                def
                    .Select(character => character.ToString())
                    .Skip(1)
                    .WhereNth(4, (s, i) => new { CargoItem = s, CharIndex = i})
                    .Where(x => !string.IsNullOrWhiteSpace(x.CargoItem))
                    .ForEach(x => stacks[x.CharIndex / 4].Push(x.CargoItem));
            }

            return stacks;
        }

        private static List<Rearrangement> LoadRearrangements(List<string> inputPart)
        {
            return inputPart
               .Select(s => s.ParseRegex(@"^move (\d+) from (\d+) to (\d+)$", m => new Rearrangement()
               {
                   Amount = int.Parse(m.Groups[1].Value),
                   From = int.Parse(m.Groups[2].Value),
                   Dest = int.Parse(m.Groups[3].Value),
               })).ToList();
        }

        public static List<List<string>> LoadInputParts(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .GroupByLineSeperator()
                .ToList();
        }
    }
}
