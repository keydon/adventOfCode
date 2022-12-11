using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    public record Monkey 
    {
        public int InspectionCount { get; set; }
        public LinkedList<long> Items { get; internal set; }
        public Func<long, long> Operation { get; internal set; }
        public int IfTrue { get; internal set; }
        public int IfFalse { get; internal set; }
        public int DividibleBy { get; internal set; }

        internal void MakeTurn(Func<long, long> relief, List<Monkey> monkeys)
        { 
            foreach (var item in Items)
            {
                InspectionCount++;
                var processed = Operation.Invoke(item);
                var reliefed = relief.Invoke(processed);
                var targetMonkey = reliefed % DividibleBy == 0 ? IfTrue : IfFalse;
                monkeys[targetMonkey].Items.AddLast(reliefed);
            }
            Items.Clear();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();

            PartOne();
            PartTwo();
            
            Report.End();
        }

        private static void PartOne(){
            var monkeys = LoadMonkeys("input.txt");
            //monkeys = LoadMonkeys("sample.txt");
            
            Enumerable.Range(1, 20)
                .SelectMany(_ => monkeys)
                .ForEach(m => m.MakeTurn(x => (int)Math.Floor(x/3d), monkeys));

            monkeys.MostActive(2)
                .MultiplyMany(m => m.InspectionCount)
                .AsResult1();
        }

        private static void PartTwo(){
            var monkeys = LoadMonkeys("input.txt");
            //monkeys = LoadMonkeys("sample.txt");
            
            var kgv = AocMath.KgV(monkeys.Select(m => m.DividibleBy).ToArray());
            Enumerable.Range(1, 10_000)
                .SelectMany(_ => monkeys)
                .ForEach(m => m.MakeTurn((x) => x % kgv, monkeys));

            monkeys.MostActive(2)
                .MultiplyMany(m => m.InspectionCount)
                .AsResult2();
        }

        public static List<Monkey> LoadMonkeys(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .ParseMonkey()
                .ToList();
        }
        
    }

    public static class Extension {
        
        public static IEnumerable<Monkey> MostActive(this IEnumerable<Monkey> monkeys, int amount){
            return monkeys
                .OrderByDescending(m => m.InspectionCount)
                .Take(amount);
        }
        public static IEnumerable<Monkey> ParseMonkey(this IEnumerable<List<string>> monkeySpecs){
            foreach (var monkeySpec in monkeySpecs)
            {
                var parts = monkeySpec.Splizz(":", " ", ",", "=").ToList();
                var monkey = new Monkey(){
                    Items = new LinkedList<long>(parts[1].Skip(2).Select(i => long.Parse(i))),
                    Operation = ParseOperation(parts[2].Skip(2).ToList()),
                    DividibleBy = int.Parse(parts[3].Last()),
                    IfTrue = int.Parse(parts[4].Last()),
                    IfFalse = int.Parse(parts[5].Last()),
                };
                yield return monkey;    
            }
        }

        private static Func<long, long> ParseOperation(List<string> list)
        {
            var op = (Left: list[0], Op: list[1], Right: list[2]);
            return op switch 
            {
                ("old", "*", string x) => (it) => it * (long.TryParse(x, out long xx) ? xx : it),
                ("old", "+", string x) => (it) => it + (long.TryParse(x, out long xx) ? xx : it),
                _ => throw new Exception("Unknown op: " + op)
            };
        }
    }
}
