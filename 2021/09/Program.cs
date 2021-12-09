using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace aoc
{
    record Location<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int Height { get; set; }
        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var locations = LoadHeightmap("input.txt");
            //foos = LoadFoos("sample.txt");
            var field = new Field<Point2, Location<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(locations);

            var lowPoints = field.AllFields.Where(f => field.GetSimpleNeighbours(f).All(n => n.Height > f.Height)).ToList();

            lowPoints
                .Select(p => p.Height + 1)
                .Sum().AsResult1();

            lowPoints
                .Select(p => BuildBasin(field, p))
                .Select(b => b.Count)
                .OrderByDescending(c => c)
                .Take(3)
                .MultiplyAll()
                .AsResult2();

            Report.End();
        }

        private static List<Location<Point2>> BuildBasin(Field<Point2, Location<Point2>> field, Location<Point2> p)
        {
            var all = new List<Location<Point2>>() { p };
            var lastExpansion = all;

            while(true) {
                var higher = lastExpansion
                    .SelectMany(f => field.GetSimpleNeighbours(f).Where(n => n.Height > f.Height && n.Height < 9))
                    .Except(all).ToList();
                if (higher.Count == 0)
                    break;
                all.AddRange(higher);
                lastExpansion = higher;
            }
            return all;
        }

        public static List<Location<Point2>> LoadHeightmap(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((p, value) => new Location<Point2> { Pos = p, Height = int.Parse(value) })
                .ToList();

            return foos;
        }
    }
}
