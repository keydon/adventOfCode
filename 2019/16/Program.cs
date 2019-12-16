using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace day16
{
    class Program
    {
        private const string input = "input.txt";
        private static readonly long[] pattern = new long[] {0, 1, 0, -1};
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
      
            var inputSignal = File.ReadAllText(input).Trim();
            for (int phase = 0; phase < 100; phase++)
            {
                inputSignal = string.Join(string.Empty, CalcPhase(inputSignal));
            }
            var solutionPartOne = string.Join(string.Empty, inputSignal.Take(8).Select(c => c.ToString()) );

    
            stopwatch.Stop();
            
            Console.WriteLine("First 8 digits: >> {0} <<", solutionPartOne);
            Console.WriteLine("Execution took: {0}", stopwatch.Elapsed);
            
            Console.WriteLine("==== Part 2 ====");
            stopwatch.Restart();

            // Dont know what mathematical mumbo jumbo is going on here, but reddit helped to implement that following shit!
            inputSignal = File.ReadAllText(input).Trim();
            var offset = int.Parse(string.Join(string.Empty, inputSignal.Take(7).Select(c => c.ToString())));
            Console.WriteLine("Offset: {0}", offset);
            
            var minimalSignal = Enumerable.Repeat(inputSignal, 10_000)
                .SelectMany(s => s)
                .Select(c => (int)char.GetNumericValue(c))
                .Skip(offset)
                .ToArray();
            
            for (int phase = 0; phase < 100; phase++)
            {
                var sum = 0;
                for (int i = minimalSignal.Length-1; i >= 0; i--)
                {
                    sum += minimalSignal[i];
                    sum = Math.Abs(sum % 10);
                    minimalSignal[i] = sum;
                }
            }
            
            var msg = string.Join(string.Empty, minimalSignal.Take(8).Select(c => c.ToString()));
            Console.WriteLine("Final message: >> {0} <<", msg);
            
            stopwatch.Stop();
            Console.WriteLine("Execution took: {0}", stopwatch.Elapsed);
        }

        private static IEnumerable<char> CalcPhase(string inputSignal)
        {
            for (int offset = 0; offset < inputSignal.Length; offset++)
            {
                yield return GetDigits(inputSignal)
                    .Zip(PatternFor(offset))
                    .Select(a => a.First * a.Second)
                    .Sum().ToString().Last();
            }
        }

        private static IEnumerable<long> PatternFor(int phase)
        {
            var dropFirst = true;

            while(true){
                foreach (var p in pattern)
                {
                    for (int i = 0; i <= phase; i++)
                    {
                        if(dropFirst){
                            dropFirst = false;
                            continue;
                        }   
                        yield return p;
                    }
                }
            }
        }

        private static IEnumerable<int> GetDigits(string inputSignal)
        {
            return inputSignal.Select(c => int.Parse(c.ToString()));
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
