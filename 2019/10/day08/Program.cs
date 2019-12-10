using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day10
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var astroidsList = LoadAsteroids("input.txt");

            CalculateAllLinesOfSight(astroidsList);

            var station = astroidsList.OrderByDescending(a => a.Visible.Count).First();
            Console.WriteLine($"{station.Point}, {station.Visible.Count}");
            Console.ReadKey();

            var map = astroidsList.Select(a => a.Point).ToMultiDimArray('#');
            station.Visible.ToMultiDimArray('█', map);
            (new List<Point>() {station.Point}).ToMultiDimArray('1', map);
            map.ToConsole();
            Console.ReadKey();
            return;



            foreach (var astroid in astroidsList.OrderByDescending(a => a.Visible.Count).Take(5))
            {
                Console.WriteLine($"{astroid.Point} {astroid.Visible.Count} / {astroid.Blocked.Count} / {astroid.Candidates.Count}");
            }

            //Console.WriteLine(string.Join(",\r\n", astroidsDic[new Point(3,4)].Debug));


            stopwatch.Stop();
            Console.WriteLine("Layer with fewest zeros: {0}", null);
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            /*


            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            stopwatch.Stop();
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();*/
        }

        public static void CalculateAllLinesOfSight(List<Astroid> astroidsList)
        {
            var grr = astroidsList.Join(astroidsList, x => true, x => true, CheckSight).ToList();
        }

        public static List<Astroid> LoadAsteroids(string inputTxt)
        {
            var astroidsDic = File
                .ReadAllLines(inputTxt)
                .SelectMany((row, y) => row.Select((astro, x) => new {astro, x, y}))
                .Where(a => a.astro == '#')
                .ToDictionary(
                    (a) => new Point(a.x, a.y),
                    (a) => new Astroid(new Point(a.x, a.y))
                );

            var astroidsPointList = astroidsDic.Select(kvp => kvp.Key).ToList();
            var astroidsList = astroidsDic.Select(kvp => kvp.Value).ToList();

            foreach (var astroid in astroidsList)
            {
                astroid.SetCandidates(astroidsPointList);
                astroid.SetAstroidDictionary(astroidsDic);
            }

            return astroidsList;
        }


        private static int CheckSight(Astroid a, Astroid b)
        {
            if (!a.Candidates.Contains(b.Point))
                return 0;
            a.Candidates.Remove(b.Point);

            if (a.Equals(b))
            {
                return 0;
            }


            a.Debug.Add($"Checking line of sight for: {a.Point} -> {b.Point}");

            //if (a.Point.Equals(new Point(0, 0)) && b.Point.Equals(new Point(6, 2)))
            //{
            //    Debugger.Break();
            //}

            var xDistance = Math.Abs(a.Point.X - b.Point.X);
            var yDistance = Math.Abs(a.Point.Y - b.Point.Y);
            var xDivisor = 1;
            var yDivisor = 1;
            if (xDistance != yDistance)
            {
                var gcd = GCD(xDistance, yDistance);
                xDivisor = xDistance / gcd;
                yDivisor = yDistance / gcd;
            }

            var xDirection = (a.Point.X == b.Point.X) ? 0 : (a.Point.X < b.Point.X) ? 1 : -1 ;
            var yDirection = (a.Point.Y == b.Point.Y) ? 0 : (a.Point.Y < b.Point.Y) ? 1 : -1 ;

            if (a.Point.X == b.Point.X) yDivisor = 1;
            if (a.Point.Y == b.Point.Y) xDivisor = 1;

            a.Debug.Add($"Step: {xDivisor * xDirection} {yDivisor * yDirection}");
            var step = BuildStep(xDivisor * xDirection, yDivisor * yDirection);

            var current = a.Point;
            var minX = Math.Min(a.Point.X, b.Point.X);
            var maxX = Math.Max(a.Point.X, b.Point.X);
            var minY = Math.Min(a.Point.Y, b.Point.Y);
            var maxY = Math.Max(a.Point.Y, b.Point.Y);
            var potentialBlockers = new List<Point>();
            do
            {
                current = step(current);
                if (current.Equals(b.Point))
                {
                    if (potentialBlockers.Count == 0)
                    {
                        a.Debug.Add($"Visible (1): {b.Point}");
                        a.Visible.Add(b.Point);
                    }
                    else
                    {
                        a.Debug.Add($"BLOCKED: {b.Point} (by {string.Join(",", potentialBlockers)})");
                        a.Blocked.Add(b.Point);
                    }

                    return 0;
                }
                if(a.AstroidsDic.TryGetValue(current, out var blockingAstroid))
                {
                    potentialBlockers.Add(current);
                }
                else
                {
                    a.Debug.Add($"{current} not blocking: {b.Point}");
                }

            } while (current != b.Point && minX <= current.X && current.X <= maxX && minY <= current.Y && current.Y <= maxY);


            a.Debug.Add($"Visible (2): {b.Point}");
            a.Visible.Add(b.Point);

            return 0;
        }

        public static int GCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            if (a == 0)
                return b;
            else
                return a;
        }

        public static int SmallestDivisor(int n, int d=2)
        {
            if (n == 0)
                return 1;
            if (n == 1)
                return 1;
            if (n == 2)
                return 1;
            if (n % d == 0)
                return d;
            return SmallestDivisor(n, ++d);
        }

        private static Func<Point,Point> BuildStep(int xStep, int yStep)
        {
            return (p) => new Point(p.X + xStep, p.Y + yStep);
        }

        public static Astroid CalcDegree(Astroid station, Astroid astroid)
        {
            var origin = station.Point;
            var target = astroid.Point;
            var degree = Math.Atan2(target.X - origin.X, target.Y - origin.Y) * (180/Math.PI);
            astroid.Degree = degree;
            return astroid;
        }
    }


    public class Astroid
    {

        protected bool Equals(Astroid other)
        {
            return Point.Equals(other.Point);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Astroid) obj);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        public Point Point { get; private set; }

        public Astroid(Point point)
        {
            this.Point = point;
            Debug = new List<string>();
        }

        public List<Point> Candidates {get; set;}
        public HashSet<Point> Visible {get; set;}
        public HashSet<Point> Blocked {get; set;}

        public List<string> Debug { get; set; }

        public void SetCandidates(List<Point> points){
            Candidates = new List<Point>(points);
            Visible = new HashSet<Point>(points.Count);
            Blocked = new HashSet<Point>(points.Count);
        }

        public void SetAstroidDictionary(Dictionary<Point, Astroid> astroidsDic)
        {
            this.AstroidsDic = astroidsDic;
        }

        public Dictionary<Point, Astroid> AstroidsDic { get; set; }
        public double Degree { get; set; }
    }
}



public static class Extensions
{
    public static T[,] ToMultiDimArray<T>(this IEnumerable<Point> points, T symbol, T[,] arr = null)
    {
        if (arr == null)
        {
            var maxY = points.Select(p => p.Y).Max();
            var maxX = points.Select(p => p.X).Max();
            arr = new T[maxX +1,maxY +1];
        }
        points.ToList().ForEach(p => arr[p.X, p.Y] = symbol);
        return arr;
    }

    public static void ToConsole<T>(this T[,] canvas, Func<T, string> tranform = null){
        var innerTransform = tranform ?? new Func<T, string>( (x) => x.ToString());

        Console.WriteLine($"\r\n[{canvas.GetUpperBound(0)+1},{canvas.GetUpperBound(1)+1}]");
        for (int y = 0; y <= canvas.GetUpperBound(1); y++)
        {
            for (int x = 0; x <= canvas.GetUpperBound(0); x++)
            {
                Console.Write(innerTransform(canvas[x,y]));
            }
            Console.Write("\r\n");
        }
        Console.Write("\r\n");
    }

    public static IEnumerable<string> Splizz(this string str, params string[] seps){
        if(seps == null || seps.Length == 0)
            seps = new[]{";", ","};
        return str.Split(seps,StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
    }
    public static IEnumerable<IEnumerable<string>> Splizz(this IEnumerable<string> enumerable, params string[] seps){
        foreach (var item in enumerable)
        {
            yield return item.Splizz(seps);
        }
    }

    public static int ToInt(this char character){
        return int.Parse(character.ToString());
    }
}
public static class EnumerableExtensions
{

  public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
  {
     if (chunkSize == 0)
        throw new ArgumentNullException();

     var enumer = source.GetEnumerator();
     while (enumer.MoveNext())
     {
        yield return Take(enumer.Current, enumer, chunkSize);
     }
  }

  private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int chunkSize)
  {
     while (true)
     {
        yield return head;
        if (--chunkSize == 0)
           break;
        if (tail.MoveNext())
           head = tail.Current;
        else
           break;
     }
  }
}


