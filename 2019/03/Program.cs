using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day03
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var allLines = File.ReadAllLines("input.txt");
            var firstWire = allLines[0];
            var secondWire = allLines[1];

            var firstFields = new List<Point>();
            var secondsFields = new List<Point>();

            walkPath(firstFields, firstWire);
            walkPath(secondsFields, secondWire);

            var crossedFields = firstFields.Intersect(secondsFields);
            var nearestCrossingPointByManhatten = crossedFields
                .Select(p => new {
                    Distance = CalcManhatten(p), 
                    Point = p
                })
                .OrderBy(c => c.Distance)
                .First();

            stopwatch.Stop();
            Console.WriteLine("Closest point by manhatten distance: {0}", nearestCrossingPointByManhatten);
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();


            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            var nearestCrossingPointByWalkinDistance = crossedFields
                .Select(p => new {
                    Distance = CalcWalkingDistance(p, firstFields, secondsFields), 
                    Point = p
                })
                .OrderBy(c => c.Distance)
                .First();

            Console.WriteLine("Closest point by walking distance", nearestCrossingPointByWalkinDistance);
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static int CalcWalkingDistance(Point point, List<Point> firstFields, List<Point> secondsFields)
        {
            return
                firstFields.TakeWhile(p => !p.Equals(point)).Count() +1
                +
                secondsFields.TakeWhile(p => !p.Equals(point)).Count() +1;
        }

        private static int CalcManhatten(Point point)
        {
            return Math.Abs(point.X) + Math.Abs(point.Y);
        }

        private static void walkPath(List<Point> fields, string wire)
        {
            var moves = wire.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s =>
                new {Direction = s[0], Distance = int.Parse(string.Join(string.Empty, s.Skip(1).ToArray()))});

            var pos = new Point(0, 0);
            foreach (var move in moves)
            {
                var step = BuildStep(move.Direction);
                for (int i = 0; i < move.Distance; i++)
                {
                    pos = step(pos);
                    fields.Add(pos);
                }
            }
        }

        private static Func<Point,Point> BuildStep(char direction)
        {
            switch (direction)
            {
                case 'L':
                    return (p) => new Point(p.X - 1, p.Y);
                case 'R':
                    return (p) => new Point(p.X + 1, p.Y);
                case 'D':
                    return (p) => new Point(p.X, p.Y - 1);
                case 'U':
                    return (p) => new Point(p.X, p.Y + 1);
            }
            throw new Exception("unknown direction " + direction);
        }
    }
}
