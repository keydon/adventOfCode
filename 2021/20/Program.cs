using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo : IHasPosition<Point2>
    {
        public string Pixel { get; set; }

        public Point2 Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var (lookup, inputPixels) = LoadFoos("input.txt");
            //var (lookup, inputPixels) = LoadFoos("sample.txt");

            var input = new Field<Point2, Foo>(OutOfBoundsStrategy.RETURN_NULL);
            Console.WriteLine("Ori");
            input.Add(inputPixels);
            input.ToConsole(x => x.Pixel);
             // 5662 too low
             // 5705 too high


            for (int i = 0; i < 50; i++)
            {
                var result = new Field<Point2, Foo>(OutOfBoundsStrategy.CREATE_NEW);
                for (int x = input.MinX-3; x < input.MaxX+4; x++)
                {
                    for (int y = input.MinY-3; y < input.MaxY+4; y++)
                    {
                        var pos = new Point2(x, y);
                        var inputSnippet = loadSnippet(input, pos);
                        result.Add(new Foo(){
                            Pos = pos,
                            Pixel = lookup[inputSnippet].ToString()
                        });
                    }
                }
                result.EmptyField = input.EmptyField == "." ? "#" : ".";
                i.Debug("Encance");
                trim(result, result.EmptyField);

                // 91166 too high
                // 19638
                //result.ToConsole(p => p.Pixel);
                input = result;
            }

            input.AllFields.Count(p => p.Pixel == "#").AsResult1();


            Report.End();
        }

        private static void trim(Field<Point2, Foo> result, string trimmer)
        {   
            var trimmed = false;
            var firstRowPoints = result.SelectEachRow(p => p).First().ToList();
            var lastRowPoints = result.SelectEachRow(p => p).Last().ToList();
            var firstColPoints = result.SelectEachCol(p => p).First().ToList();
            var lastColPoints = result.SelectEachCol(p => p).Last().ToList();
            var borders = new[] {firstRowPoints, lastRowPoints, firstColPoints, lastColPoints};
            foreach (var border in borders)
            {
                if(border
                    .Select(p => result.GetOrElse(p, (v) => v.Pixel, _ => trimmer))
                    .All(p => p == trimmer)){
                        //Console.WriteLine("triiiiiim");
                    //border.ForEach(p => result.Remove(p));
                    result.BulkTrim(border);
                    trimmed = true;
                }
            }

            if(trimmed) trim(result, trimmer);
        }

        private static int loadSnippet(Field<Point2, Foo> input, Point2 point2)
        {
            var n = point2.GetNeighbours().ToList();
            n.Add(point2);
            var key = n
                .OrderBy(p => p.Y)
                .ThenBy(p => p.X)
                .Select(p => input.GetOrElse(p, (v) => v.Pixel, _ => input.EmptyField))
                .Select(c => c == "#" ? "1" : "0")
                .ToCommaString("")
                .FromBinaryToLong();
            return (int) key;
        }

        public static (Dictionary<int, char> lookup, List<Foo> pixels) LoadFoos(string inputTxt)
        {
            var parts = File
                .ReadAllLines(inputTxt)
                //.Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             .GroupByLineSeperator()
             .ToList();

            var lookup = parts.First().First().Select((c, i) => (c, i)).ToDictionary((t) => t.i, (t) => t.c);

            var pixels = parts.Last()
            .Parse2DMap((p, t) => new Foo { Pos = p, Pixel = t })
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

            return (lookup, pixels);
        }
    }
}
