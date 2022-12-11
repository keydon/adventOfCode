using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Addition
    {
        public int CycleLength { get; set; }
        public int Parameter { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var compactAdditions = LoadProgram("input.txt");
            //compactInstrcompactAdditionsuctions = LoadProgram("sample.txt");

            var timedAdditions = compactAdditions.SelectMany(f => { 
                    var expanded = Enumerable.Range(1, f.CycleLength-1).Select(_ => 0).ToList();
                    expanded.Add(f.Parameter);
                    return expanded;
                });

            var requestedCycles = new List<int>(){ 20, 60, 100, 140, 180, 220 };
            requestedCycles.Select(cycle => cycle * CalcRegisterForCycle(timedAdditions, cycle))
                .Sum()
                .AsResult1();

            var height = 6;
            var width = 40;
            Points.GenerateGrid(width, height)
                .Select(p => (point: p, cycle: p.Y * width + p.X + 1))
                .Select(p => (p.point, register: CalcRegisterForCycle(timedAdditions, p.cycle)))
                .Where(p => IsInSprite(p))
                .Select(p => p.point)
                .ToConsole();

            Report.End();
        }

        private static long CalcRegisterForCycle(IEnumerable<int> timedAdditions, int cycle)
        {
            var register = 1 + timedAdditions.Take(cycle-1).Sum();
            return register;
        }

        private static bool IsInSprite((Point2 point, long register) p)
        {
            var x = p.point.X;
            var start = p.register - 1;
            var end = p.register + 1;
            return start <= x && x <= end;
        }

        public static List<Addition> LoadProgram(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^([a-z]+)( ([-0-9+]+))?$", m => new Addition()
                {
                    CycleLength = m.Groups[1].Value switch
                    {
                        "addx" => 2,
                        "noop" => 1,
                        _ => throw new NotImplementedException(m.Groups[1].Value)
                    },
                    Parameter = int.TryParse(m.Groups[2].Value, out int parsed) 
                        ? parsed : 0,
                }))
                .ToList();
        }
    }
}
