using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc
{
    record Foo : IHasPosition<Point2>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public Point2 Pos { get; set; }

        public IEnumerable<Foo> GetNeighbours(Field<Point2, Foo> field)
        {
            var dirs = new[] { "sw", "se", "nw", "ne", "e", "w" };
            var neighs = dirs.Select(d => Program.DirectoToMove(d, this.Pos))
            .Select(m => m(this.Pos))
            .Select(p => field.GetNew(p, f => f.A = ".")).ToList();
            return neighs;
        }

        public void Calc(Field<Point2, Foo> field)
        {
            var neighs = GetNeighbours(field);

            var active = neighs.Where(f => f.A == "#").Count();
            if (string.IsNullOrWhiteSpace(A)) throw new Exception("WTF");
            B = (A, active) switch
            {
                (".", 2) => "#",
                ("#", 1) => "#",
                ("#", 2) => "#",
                ("#", _) => ".",
                (_, _) => A
            };
            //B.Debug((A, active));

        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            // foos = LoadFoos("sample.txt");
            //foos.Select(s => s.ToCommaString(",")).Peek().First().Debug("a").Assert("e,e,e,e,e,se,e,nw,se,sw");
            //var str = foos.Select(f => f.ToCommaString(""));
            //File.WriteAllLines("test.txt", str);
            var field = new Field<Point2, Foo>(OutOfBoundsStrategy.CREATE_NEW);
            var root = new Foo() { A = ".", Pos = new Point2(0, 0) };
            field.Add(root);
            foreach (var line in foos)
            {
                var pos = root.Pos;
                foreach (var item in line)
                {

                    Func<Point2, Point2> dir = DirectoToMove(item, pos);
                    pos = dir(pos);
                }
                var tile = field.GetNew(pos, f => f.A = ".");
                if (tile.A == "#")
                {
                    tile.A = ".";
                    tile.Pos.Debug("GOES WHITE AGAIN");
                }
                else
                {
                    tile.A = "#";
                }

            }

            //tile.A = tile.A == "." ? "#" : ".";
            field.ToConsole(a => a.A);
            field.AllFields.Count.Debug("AllFields");
            field.AllFields.Where(f => f.A == "#").Count().AsResult1();

            int days = 100;
            for (int i = 1; i <= days; i++)
            {
                i.Debug("day");
                field.AllFields.Where(x => x.A == "#").ToList().Select(s => s.GetNeighbours(field)).Count().Debug("grown");

                var relevant = field.AllFields.ToList();
                foreach (var item in relevant)
                {
                    item.Calc(field);
                }
                foreach (var item in relevant)
                {
                    item.A = item.B;
                }
            }

            field.AllFields.Where(f => f.A == "#").Count().AsResult2();


            Report.End();
        }

        public static Func<Point2, Point2> DirectoToMove(string item, Point2 pos)
        {
            var even = Math.Abs(pos.Y) % 2 == 0;
            return item switch
            {
                "sw" => even ? p => p.Down().Left() : p => p.Down(),
                "se" => even ? p => p.Down() : p => p.Down().Right(),
                "nw" => even ? p => p.Up().Left() : p => p.Up(),
                "ne" => even ? p => p.Up() : p => p.Up().Right(),
                "e" => p => p.Right(),
                "w" => p => p.Left(),

                _ => throw new Exception("unknonw," + item)
            };
        }

        public static List<List<string>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => Regex.Match(s, @"^((se)|(sw)|(nw)|(ne)|(e)|(w))+$"))
                .Select(s => s.Groups.Values.Skip(1).Take(1).Select(c => c.Captures.Select(c => c.Value).ToList()).SelectMany(s => s).ToList())

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
