using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace day04
{
    class Program
    {
        private const string input = "input.txt";
        private const string sample01 = "sample01.txt";
        private const string sample02 = "sample02.txt";
        private const string sample03 = "sample03.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();

            List<Point3> moons = File.ReadAllLines(sample02)
                .RegExParse<Point3>(@"<x=(?<x>-?[0-9]+), y=(?<y>-?[0-9]+), z=(?<z>-?[0-9]+)>")
                .Select((p,i) => {
                    p.Velocity = new Point3();
                    p.Index = i;
                    p.Original = p.Clone();
                    return p;
                })
                .ToList();

            var originalMoons = moons.Select(m => m.Clone()).ToList();

            var pairs = moons
                .SelectMany(a => moons.Select(b => new { a, b }))
                .Where(p => p.a != p.b)
                .ToList();

            var steps = 100;
            for (int i = 0; i < steps; i++)
            {
                pairs.ForEach(p => CalcVelo(p.a, p.b));
                moons.ForEach(ApplyVelocity);
            }

            Console.WriteLine(">> total energy: {0} <<", moons.Sum(m => m.CalcEnergy()));
            stopwatch.Stop();
            Console.WriteLine("Execution took: {0}", stopwatch.Elapsed);
            //Console.ReadKey();


            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            moons = File.ReadAllLines(sample02)
                .RegExParse<Point3>(@"<x=(?<x>-?[0-9]+), y=(?<y>-?[0-9]+), z=(?<z>-?[0-9]+)>")
                .Select((p,i) => {
                    p.Velocity = new Point3();
                    p.Index = i;
                    p.Original = p.Clone();
                    return p;
                })
                .ToList();

            originalMoons = moons.Select(m => m.Clone()).ToList();

            pairs = moons
                .SelectMany(a => moons.Select(b => new { a, b }))
                .Where(p => p.a != p.b)
                .ToList();

            steps = 0;
            while(true)
            {

                pairs.ForEach(p => CalcVelo(p.a, p.b));
                moons.ForEach(ApplyVelocity);
                ++steps;

                moons.ForEach(p => p.RememberZeroPos(steps));

                if (moons.All(m => m.Done))
                {
                    Console.WriteLine("all Cycles are repeating!");
                    Console.WriteLine(string.Join(", \r\n", moons));
                    Console.WriteLine(string.Join(", ", moons.Select(m => m.PastPositions.Count)));
                    break;
                }

            }


           // 332329817058642408

            var lcm = LCM(moons
                    .Select(m => m.Steps)
                    .Select(c => (long)c)
                    .ToArray()
                );
            Console.WriteLine(">> LCM: {0} <<", lcm);
            if (lcm == 4686774924){
                Console.WriteLine("seems legit!");
            }
            stopwatch.Stop();
            Console.WriteLine("Execution took: {0}", stopwatch.Elapsed);
        }

        static long LCM(long[] numbers)
        {
            return numbers.Aggregate(lcm);
        }
        static long lcm(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }
        static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        private static void ResetVelocity(Point3 moon)
        {
            moon.Velocity = new Point3();
        }

        private static void ApplyVelocity(Point3 moon)
        {
            moon.X += moon.Velocity.X;
            moon.Y += moon.Velocity.Y;
            moon.Z += moon.Velocity.Z;
        }

        private static void CalcVelo(Point3 a, Point3 b)
        {
            if(a.X == b.X) {
                // NOOP
            } else if (a.X < b.X){
                a.Velocity.X ++;
                b.Velocity.X --;
            }
            
            if(a.Y == b.Y) {
                // NOOP
            } else if (a.Y < b.Y){
                a.Velocity.Y ++;
                b.Velocity.Y --;
            }

            if(a.Z == b.Z) {
                // NOOP
            } else if (a.Z < b.Z){
                a.Velocity.Z ++;
                b.Velocity.Z --;
            }
        }

        private static bool hasAdjesent(int arg)
        {
            var str = arg.ToString();
            for (int i = 0; i < str.Length-1; i++)
            {
                var a = str[i];
                var b = str[i+1];

                if(b == a) {
                    var preA = i>0 ? str[i-1] : 'x';
                    var afterB = i+1 < str.Length-1 ? str[i+2] : 'x';
                    //Console.WriteLine($"{i}:\t{preA} <{a} {b}> {afterB}");
                    if(a != preA && b != afterB){
                        return true;
                    }
                }
                    
            }
            return false;
        }

        private static bool isNotDecreasing(int arg)
        {
            var str = arg.ToString();
            for (int i = 0; i < str.Length-1; i++)
            {
                var a = str[i];
                var b = str[i+1];

                if(b < a)
                    return false;
            }
            return true;
        }
    }

    public class Point3
    {

        public long X { get; set; }
        public long Y {get; set;}
        public long Z {get; set;}

        public Point3 Velocity {get; set;}
        public bool Done { get; internal set; }
        public int Steps { get; private set; }
        public int Index { get; internal set; }
        public Point3 Original { get; internal set; }

        public HashSet<Point3> PastPositions = new HashSet<Point3>();

        public override bool Equals(object obj)
        {
            return obj is Point3 point &&
                   X == point.X &&
                   Y == point.Y &&
                   Z == point.Z && (Velocity?.Equals(point.Velocity)??true);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, Velocity);
        }

        
        public long CalcEnergy(){
            var pos = CalcPosEnergy();
            if(Velocity != null){
                pos*= CalcKineticEnergy();
            }
            return pos;
        }

        public long CalcPosEnergy(){
            return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }

        public long CalcKineticEnergy(){
            if(Velocity != null){
                return Velocity.CalcEnergy();
            }
            return 0;
        }

        public Point3 Clone(){
            var clone = (Point3)this.MemberwiseClone();
            clone.Velocity = new Point3();
            return clone;
        }

        public override string ToString(){
            return $"{X} {Y} {Z} -> {Velocity?.X} {Velocity?.Y} {Velocity?.Z}";
        }

        internal void RememberZeroPos(int steps)
        {
            if(Done) 
                return;
            if(this.Equals(Original)) {
                Console.WriteLine("One moon down {0}", Index);
                Done = true;
                Steps = steps;
            };
        }
    }

    public static class Extensions
    {
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
         public static IEnumerable<IEnumerable<string>> Splizz(this string[] enumerable, params string[] seps){
            foreach (var item in enumerable)
            {
                yield return item.Splizz(seps);
            }
        }

        public static IEnumerable<T> Many<T>(this IEnumerable<IEnumerable<T>> enumerable){
            foreach (var item in enumerable)
            {
                foreach (var nestedItem in item)
                {
                     yield return nestedItem;
                }
            }
        }

        public static string ToCommaString<T>(this IEnumerable<T> enumerable, string seperator = ", "){
            return string.Join(seperator, enumerable.Select(e => e.ToString()));
        }

        public static int ManhattenDistance(this Point point, Point other){
            return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
        } 

        public static IEnumerable<T> RegExParse<T>(this IEnumerable<string> enumerable, string pattern)
            where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).ToArray();
            var regex = new Regex(pattern, RegexOptions.Compiled);
            foreach (var item in enumerable)
            {
                Match match = regex.Match(item);
                var t = new T();
                foreach (var group in match.Groups.Keys.Where(k => !int.TryParse(k, out var _)))
                {
                    var capture = match.Groups[group];
                    var prop = props.FirstOrDefault(p => string.Equals(p.Name, group, StringComparison.OrdinalIgnoreCase));
                    if(prop == null)
                        throw new Exception($"Property '{group}' not found on type '{type.Name}', candidates were {props.ToCommaString()}");

                    prop.SetValue(t, Convert.ChangeType(capture.Value, prop.PropertyType));  
                }

                yield return t;
            }
        }
    }
}
