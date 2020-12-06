using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;


namespace _05
{
    record Seat
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int ID { get; set; }
        public Point Position { get; set; }
    }
    class Program
    {
        public static int CalculationPosition(string boardingpass, char lowerPartionInstruction, int min, int max)
        {
            foreach (var partitionInstruction in boardingpass)
            {
                if (partitionInstruction == lowerPartionInstruction)
                {
                    max = (max + min) / 2;
                }
                else
                {
                    min += (max - min + 1) / 2;
                }
            }
            var position = min;
            return position;
        }

        public static Seat CalculateSeat(string pass)
        {
            var row = CalculationPosition(pass[0..^3], 'F', 0, 127);
            var col = CalculationPosition(pass[^3..], 'L', 0, 7);
            var id = (row * 8) + col;

            return new Seat()
            {
                Row = row,
                Col = col,
                ID = id,
                Position = new Point(row, col)
            };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var boardingPasses = LoadBoardingPasses("input.txt");

            var result1 = boardingPasses.Select(f => CalculateSeat(f)).Max(s => s.ID);

            Console.WriteLine($"Part1-Result: {result1}");


            Console.WriteLine("==== Part 2 ====");
            /* This solution would have been easier, but at first I assumed that IDs are not consecutive ;O
            var ids = boardingPasses.Select(s => CalculateSeat(s).ID).ToList();
            var min = ids.Min();
            var max = ids.Max();
            var myId = Enumerable.Range(min, max - min).Except(ids).Single();
            Console.WriteLine($"Part2-Result: {myId}");
            */

            var field = new Field<Seat>(f => f.Position);
            field.Add(boardingPasses.Select(f => CalculateSeat(f)));
            var allSeats = Enumerable.Range(1, 128).SelectMany(row => Enumerable.Range(0, 8).Select(col => new Seat()
            {
                Row = row,
                Col = col,
                ID = (row * 8) + col,
                Position = new Point(row, col)
            }));

            var freeSeats = allSeats.Select(s => s.Position).Except(field.AllFields.Select(s => s.Position));
            var withNoFreeNeighbour = freeSeats.Where(s => s.GetNeighbours().All(n => !freeSeats.Contains(n)));
            var mySeatPosition = withNoFreeNeighbour.Single();
            var mySeatId = (mySeatPosition.X * 8) + mySeatPosition.Y;

            field.ToConsole(s => "#");

            Console.WriteLine($"Part2-Result: {mySeatId}");
        }

        public static List<string> LoadBoardingPasses(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim());

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
        }
    }

    public class Field<T>
    {
        public string EmptyField { get; set; } = ".";
        public int MaxY { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MinX { get; private set; }

        public R GetOrElse<R>(Point p, Func<T, R> extractor, R elze)
        {
            if (Dic.TryGetValue(p, out var v))
            {
                return extractor(v);
            }
            return elze;
        }
        public readonly Dictionary<Point, T> Dic = new Dictionary<Point, T>();
        public readonly LinkedList<T> AllFields = new LinkedList<T>();
        private readonly Func<T, Point> pointGetter;
        public Field(Func<T, Point> PointGetter)
        {
            pointGetter = PointGetter;
        }

        public void Add(T item)
        {
            var p = pointGetter(item);
            if (p.X > MaxX)
            {
                MaxX = p.X;
            }
            if (p.X < MinX)
            {
                MinX = p.X;
            }
            if (p.Y > MaxY)
            {
                MaxY = p.Y;
            }
            if (p.Y < MinY)
            {
                MinY = p.Y;
            }
            Dic[p] = item;
            AllFields.AddLast(item);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var f in items)
            {
                Add(f);
            }
            Console.WriteLine($"maxX: {MaxX}, maxY: {MaxY}, total: {AllFields.Count}");
        }

        public IEnumerable<IEnumerable<R>> SelectEachRow<R>(Func<Point, R> extractor)
        {
            Console.WriteLine($"[{MinX},{MinY}]x[{MaxX},{MaxY}]");
            return Enumerable.Range(MinY, MaxY - MinY + 1)
                .Select(y =>
                    Enumerable.Range(MinX, MaxX - MinX + 1)
                        .Select(x => new Point(x, y))
                        .Select(extractor));
        }

        public IEnumerable<T> GetNeighbours(Point p)
        {
            return p.GetNeighbours()
                .Where(Dic.ContainsKey)
                .Select(p => Dic[p]);
        }

        public void ToConsole(Func<T, string> printer)
        {
            var lines = SelectEachRow(p => GetOrElse<string>(p, printer, EmptyField))
                .Select(row => row.ToCommaString(""));
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
    public static class EnumerableExtensions
    {
        public static string ToCommaString<T>(this IEnumerable<T> items, string delimiter = ",")
        {
            return string.Join(delimiter, items.Select(i => i.ToString()));
        }
    }
    public static class PointExtensions
    {
        public static Point Left(this Point p, int step = 1) => new Point(p.X - step, p.Y);
        public static Point Up(this Point p, int step = 1) => new Point(p.X, p.Y - step);
        public static Point Right(this Point p, int step = 1) => new Point(p.X + step, p.Y);
        public static Point Down(this Point p, int step = 1) => new Point(p.X, p.Y + step);

        public static IEnumerable<Point> GetNeighbours(this Point p)
        {
            yield return p.Up();
            yield return p.Left();
            yield return p.Right();
            yield return p.Down();
        }
    }
}
