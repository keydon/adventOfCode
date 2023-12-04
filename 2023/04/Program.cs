using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo
    {
        public int X { get; set; }
        public List<int> W { get; set; }

        public List<int> H { get; set; }
        public string B { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            foos.Select(c => c.H.Intersect(c.W).Count())
                .Where(c => c > 0)
                .Select(c => c == 1 ? 1 : Math.Pow(2, c-1))
                .Sum().AsResult1();


            Report.End();
        }

        public static List<Foo> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             //.GroupByLineSeperator()
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             .Select(s => s.ParseRegex(@"^Card (.+): (.+) \| (.+)$", m => new Foo()
               {
                   X = int.Parse(m.Groups[1].Value),
                   W = m.Groups[2].Value.Splizz(" ").Select(int.Parse).ToList(),
                   H = m.Groups[3].Value.Splizz(" ").Select(int.Parse).ToList(),
                   
               }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
             .ToList()
            ;

            return foos;
        }
    }
}
