using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    enum Ori
    {
        x0, x0r, y0, y0r, xM, xMR, yM, yMR
    }
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
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            foreach (var f in foos)
            {
                f.Borders = CalcBorders(f);
            }
            var fieldDic = foos.ToDictionary(f => f.Id);

            var borderDic = new Dictionary<string, List<Field<Point2, Foo<Point2>>>>();
            foreach (var f in foos)
            {
                MatchBorders(borderDic, f, foos);
            }
            var bc = foos.Select(f =>
                  (f.Borders
                      .Select(b => b.border)
                      .Select(b => borderDic[b].Count)
                      .Sum(), f.Id)
                );
            var min = bc.Min(p => p.Item1);
            var max = bc.Max(p => p.Item1);
            var mins = bc.Where(b => b.Item1 == min).ToList();
            if (mins.Count == 4)
            {
                mins.Select(p => p.Id).MultiplyAll().AsResult1();
            }

            var bigTile = BuildBigTile(borderDic, fieldDic, foos, fieldDic[mins.First().Id]);




            Report.End();
        }

        private static object BuildBigTile(Dictionary<string, List<Field<Point2, Foo<Point2>>>> borderDic, Dictionary<int, Field<Point2, Foo<Point2>>> fieldDic, List<Field<Point2, Foo<Point2>>> foos, Field<Point2, Foo<Point2>> field)
        {
            var bigTile = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            bigTile.Add(StripBorder(field));
        }

        private static Field<Point2, Foo<Point2>> StripBorder(Field<Point2, Foo<Point2>> f)
        {

            var minx

            var field = new Field<Point2, Foo<Point2>>(f.OutOfBoundsStrategy);
            for (int x = f.MinX; x <= f.MaxX; x++)
            {
                for (int y = f.MinY; y <= f.MaxY; y++)
                {
                    if (x == 0 || y == 0 || x == f.MaxX || y == f.MaxY)
                    {
                        continue;
                    }
                    else
                    {
                        var foo = f.Dic[new Point2(x, y)];
                        yield return foo;
                    }
                }
            }
        }

        private static object MatchBorders(Dictionary<string, List<Field<Point2, Foo<Point2>>>> dic, Field<Point2, Foo<Point2>> f, List<Field<Point2, Foo<Point2>>> foos)
        {
            foreach (var b in f.Borders)
            {
                var bs = b.border;
                if (dic.TryGetValue(bs, out var bl))
                {
                    bl.Add(f);
                }
                else
                {
                    dic.Add(bs, new List<Field<Point2, Foo<Point2>>>() { f });
                }
            }
            return dic;
        }

        private static List<(string border, Ori orientation)> CalcBorders(Field<Point2, Foo<Point2>> f)
        {
            var x0 = new List<string>();
            var y0 = new List<string>();
            var xM = new List<string>();
            var yM = new List<string>();

            for (int x = f.MinX; x <= f.MaxX; x++)
            {
                for (int y = f.MinY; y <= f.MaxY; y++)
                {
                    var foo = f.Dic[new Point2(x, y)];
                    if (x == 0)
                    {
                        x0.Add(foo.A);
                    }
                    if (y == 0)
                    {
                        y0.Add(foo.A);
                    }
                    if (x == f.MaxX)
                    {
                        xM.Add(foo.A);
                    }
                    if (y == f.MaxY)
                    {
                        yM.Add(foo.A);
                    }
                }
            }
            var res = new List<(string border, Ori orientation)>
            {
                (x0.ToCommaString(""), Ori.x0),
                (x0.Select(s => s).Reverse().ToCommaString(""), Ori.x0r),
                (y0.ToCommaString(""), Ori.y0),
               (y0.Select(s => s).Reverse().ToCommaString(""), Ori.y0r),

                (xM.ToCommaString(""), Ori.xM),
               (xM.Select(s => s).Reverse().ToCommaString(""), Ori.xMR),

                (yM.ToCommaString(""), Ori.yM),
               (yM.Select(s => s).Reverse().ToCommaString(""), Ori.yMR),
            };
            return res.Debug(f.Id);
        }

        public static List<Field<Point2, Foo<Point2>>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .Select(g =>
                {
                    var id = int.Parse(g.First().Splizz(" ").Select(s => s.Trim(':')).Last());
                    var field = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
                    field.Add(g.Skip(1).Parse2DMap((p, t) => new Foo<Point2>() { A = t, Pos = p }));
                    field.Id = id;
                    return field;
                })

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
