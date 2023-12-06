using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Race 
    {
        
        public int No { get; set; }
        public int Time { get; set; }
        public int Distance { get; set; }

        internal IEnumerable<Race> SelectPossibleCombinations()
        {
            return Enumerable.Range(0, Time)
                .Select(t => new Race(){
                     No = No, Time = t, Distance = (t*(Time-t)*1) })
                .Where(r => r.Distance > Distance);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var races = LoadFoos("input.txt");
            //races = LoadFoos("sample.txt");
            races.Debug("Races").ToList();

            races.Select(
                r => r.SelectPossibleCombinations().Count()
                )
                .Debug()
                .MultiplyAll().AsResult1();

            Report.End();
        }

        public static IEnumerable<Race> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim());

            var times = foos.First().Splizz(":", " ").Skip(1).ToList();
            var distances = foos.Last().Splizz(":", " ").Skip(1).ToList();
            var races = times.Zip(distances).Select((z, i) => new Race() {
                Time = int.Parse(z.First),
                Distance = int.Parse(z.Second),
                No = i+1
            });
            return races;

             //.GroupByLineSeperator()
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
             //      A = m.Groups[1].Value,
             //      B = m.Groups[2].Value,
             //  }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
            // (()).ToList()
            //;

            //return foos;
        }
    }
}
