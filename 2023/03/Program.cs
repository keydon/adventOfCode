using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var nonSymbols = new[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "."};
            var nums = new[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");
            var field = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(foos);
            var adj = field.AllFields.Where(f => !nonSymbols.Contains(f.A)).Peek().ToList();
            var partnums = new List<long>();
            foreach (var item in adj)
            {
                var ns = field.GetNeighbours(item)
                    .Where(a => nums.Contains(a.A)).ToList();
                    ns.Select(x => x.A).ToCommaString();
                var l = ns.Count;
                if(l == 0) continue;
                while(true){
                    var ns2 = ns.SelectMany(itemz => field.GetNeighbours(itemz))
                        .Where(n => nums.Contains(n.A))
                        .Distinct()
                        .ToList();
                    ns2.AddRange(ns);
                    ns2 = ns2.Distinct().ToList();

                    if(l == ns2.Count){
                        var grp = ns2.GroupBy(k => k.Pos.Y, v => v);

                        foreach (var g in grp.SelectMany(x => SplitByX(x.ToList())))
                        {
                           var partstr = g.OrderBy(x => x.Pos.X)
                                .Select(x => x.A)
                                .ToCommaString("");
                            partnums.Add(long.Parse(partstr));
                        }
                        break;
                    }
                    ns = ns2;
                    ns.Select(x => x.A).ToCommaString();
                    l = ns.Count;
                }   
            }
            field.ToConsole((a) => a.A);
            partnums.Sum().AsResult1();
            Report.End();
        }

        private static IEnumerable<List<Foo<Point2>>> SplitByX(List<Foo<Point2>> list)
        {
            var grp = new LinkedList<Foo<Point2>>();
            var sorted = list.OrderBy(x => x.Pos.X);
            foreach (var n in sorted)
            {
                if (grp.Count == 0){
                    grp.AddLast(n);
                    continue;
                }
                if ((n.Pos.X - grp.Last().Pos.X) == 1){
                    grp.AddLast(n);
                    continue;
                }
                yield return grp.ToList();
                grp = new LinkedList<Foo<Point2>>();
                grp.AddLast(n);
            }
            if(grp.Count > 0)
            yield return grp.ToList();
        }

        public static List<Foo<Point2>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //

             //.GroupByLineSeperator()
             
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
             .ToList()
            ;

            return foos;
        }
    }
}
