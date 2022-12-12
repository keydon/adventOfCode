using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public string Name { get; set; }
        public TPoint Pos { get; set; }
        public int Height { get; internal set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var squares = LoadHeightMap("input.txt");
            //squares = LoadHeightMap("sample.txt");

            var start = squares.Where(sq => sq.Name == "S").Single().Debug();
            var end = squares.Where(sq => sq.Name == "E").Single().Debug();

            var field = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            squares.ForEach(sq => field.Add(sq));
            // field.ToConsole(p => p.Name.ToString());

            var startingFront1 = new List<Foo<Point2>>() { start };
            FindPath(startingFront1, field, end).AsResult1();
            
            var startingFront2 = squares.Where(f => f.Height == 'a').ToList();
            FindPath(startingFront2, field, end).AsResult2();

            Report.End();
        }

        private static int FindPath(List<Foo<Point2>> startingFront, Field<Point2, Foo<Point2>> field, Foo<Point2> end)
        {
            var step = 0;
            var currentFront = startingFront;
            var visited = new HashSet<Foo<Point2>>(startingFront);

            while (true)
            {
                step++;
                var nextFront = currentFront.SelectMany(f =>
                    field.GetSimpleNeighbours(f)
                        .Where(n => n.Height <= f.Height + 1)
                        .Where(n => visited.Add(n)))
                        .ToList();
                        
                if (nextFront.Contains(end))
                {
                    break;
                }
                currentFront = nextFront;
            }
            return step;
        }

        public static List<Foo<Point2>> LoadHeightMap(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((pos, tile) => new Foo<Point2> { 
                    Pos = pos, 
                    Name = tile, 
                    Height = tile switch { "S" => 'a', "E" => 'z', _ => tile[0] } })
                .ToList();
        }
    }
}
