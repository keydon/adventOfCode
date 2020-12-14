using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextCopy;
using System.Threading;

namespace aoc
{

    record Foo
    {
        public int Minutes { get; set; }
        public int Deperature { get; set; }
        public int Id { get; set; }
        public string A { get; set; }
        public string B { get; set; }

        public Point Pos { get; set; }

        public int calc()
        {
            return Minutes + Id;
        }
        public bool ist()
        {
            return A == B;
        }

        public string schtring()
        {
            return A + B;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var earliestts = 1000052;
            //earliestts = 939;
            earliestts.Debug("ts");
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");
            Foo foo = foos.Select(id => new Foo() { Deperature = ((earliestts / id) + 1) * id, Id = id })
            .Select(x =>
            {
                x.Minutes = x.Deperature - earliestts;
                return x;
            })
            .OrderBy(s => s.Minutes).First();
            foo.Debug();
            var x = foo.Id * foo.Minutes;
            // x.AsResult1();

            AocMath.GgT(156, 66).Debug("ggt");

            crs(LoadFoos2("sample.txt")).Assert(1068781L);
            crs(LoadFoos2("sample2.txt")).Assert(3417L);
            crs(LoadFoos2("sample3.txt")).Assert(754018L);
            crs(LoadFoos2("sample4.txt")).Assert(779210L);
            crs(LoadFoos2("sample5.txt")).Assert(1261476L);


            crs(LoadFoos2("sample6.txt")).Assert(1202161486L);

            crs(LoadFoos2("input.txt")).Debug("crs (input)").AsResult2();


            return;
            /*



            var f2Sorted = foos2.Skip(1).OrderByDescending(f => f.ID).ToList().Debug("sorted");

            foos2.Select(f => $"(x + {f.Offset}) % {f.ID}").ToCommaString(" + ").Debug("0 = ");

            var max = foos2.Max(f => f.ID).Debug("max");
            var maxFoo = foos2.First(f => f.ID == max);

            var first = foos2.First().ID;
            GenerateDeparturesFor(first, maxFoo)
                .AsParallel()
                .Where(d => f2Sorted.Skip(1).All(f => (d + f.Offset) % f.ID == 0))
                .First()
                .AsResult2();

            //var field = new Field<Foo>(f => f.Pos);
            //field.Add(foos.Where(f => f.A != "."));


            //field.ToConsole(f => f.A); 
*/

            Report.End();
        }

        private static object crs(List<IdOffs> busses)
        {
            var N = busses.Select(f => (long)f.ID).Aggregate((a, b) => a * b);
            N.Debug("N");
            foreach (var bus in busses)
            {
                bus.N = N / (long)bus.ID;
            }
            foreach (var bus in busses)
            {
                (_, var z1, var z2) = AocMath.Euklid(bus.N, bus.ID);
                bus.A = z1;
            }
            var sum = busses.Select(b => b.Offset * b.N * b.A).Sum();

            sum = Math.Abs(sum);
            (sum % N).Debug("most likly the solution, if not try the result below xD");
            return N - (sum % N);
        }

        private static long NewMethod(List<IdOffs> f2Sorted)
        {
            long multi = 0;
            int[] ids = f2Sorted.Select(f => f.ID).ToArray();
            int[] offsets = f2Sorted.Select(f => f.Offset).ToArray();

            int[] reloffsets = new int[4] {
                        offsets[0] - offsets[1],
                        offsets[1] - offsets[2],
                        offsets[2] - offsets[3],
                        offsets[3] - offsets[4],
                      //  offsets[4] - offsets[5],
                      //  offsets[5] - offsets[6],
                      //  offsets[6] - offsets[7],
                      //  offsets[7] - offsets[8]
                    };
            reloffsets.ToList().Debug("reloffs");
            while (true)
            {
                multi += ids[0];
                var t0 = multi - reloffsets[0];
                if (t0 % ids[1] > 0)
                    break;
                var t1 = t0 - reloffsets[1];
                if (t1 % ids[2] > 0)
                    break;
                var t2 = t1 - reloffsets[2];
                if (t2 % ids[3] > 0)
                    break;
                var t3 = t2 - reloffsets[3];
                if (t3 % ids[4] == 0)
                {
                    multi.Debug("multi");
                    return t3;
                }
            }
            return -1;
        }




        public static IEnumerable<long> GenerateDeparturesFor(int id, IdOffs max)
        {
            long departure = 0;
            while (true)
            {
                departure += max.ID;
                if ((departure - max.Offset) % id == 0)
                {
                    yield return departure - max.Offset;
                }
            }
        }

        public static IEnumerable<long> MultipleOf(int number)
        {
            long multiple = 0;
            while (true)
            {
                multiple += number;
                yield return multiple;
            }
        }

        public static List<int> LoadFoos(string inputTxt, int top = 0)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                //.SelectMany((row, y) => row.Select((foo, x) => new Foo { Pos = new Point(x, y), A = foo.ToString() }))
                .SelectMany(r => r.Splizz(",", ";"))

                .Where(a => a != "x")
                .Select(int.Parse).ToArray()
                //.Select(s => ParseRegex(s))
                //.Select(int.Parse)
                //.ToDictionary(
                //    (a) => new Point(a.x, a.y),
                //    (a) => new Foo(new Point(a.x, a.y))
                //);
                //.Where(f = f)
                ;

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
            //return foos;
        }
        public static List<IdOffs> LoadFoos2(string inputTxt, int top = 0)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                //.SelectMany((row, y) => row.Select((foo, x) => new Foo { Pos = new Point(x, y), A = foo.ToString() }))
                .SelectMany(r => r.Splizz(",", ";"))
                .Select((x, i) => new IdOffs() { Id = x, Offset = i })
                .Where(it => it.Id != "x")
                .Select(it =>
                {
                    it.ID = int.Parse(it.Id);
                    return it;
                })
                //.Select(s => ParseRegex(s))
                //.Select(int.Parse)
                //.ToDictionary(
                //    (a) => new Point(a.x, a.y),
                //    (a) => new Foo(new Point(a.x, a.y))
                //);
                //.Where(f = f)
                ;

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
            //return foos;
        }
        private static Foo ParseRegex(string line)
        {
            // 
            //
            // 
            Regex operationRegEx = new Regex(@"^(\d+) (\d+)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);

            return new Foo()
            {
                Minutes = int.Parse(match.Groups[1].Value),
                Id = int.Parse(match.Groups[2].Value),
                A = match.Groups[1].Value,
                B = match.Groups[2].Value
            };
        }
    }

    internal record IdOffs
    {
        internal int ID;
        public long N { get; set; }

        public IdOffs()
        {
        }

        public string Id { get; set; }
        public int Offset { get; set; }
        public long A { get; internal set; }
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

        public void Add(params T[] items)
        {
            foreach (var f in items)
            {
                Add(f);
            }
            Console.WriteLine($"maxX: {MaxX}, maxY: {MaxY}, total: {AllFields.Count}");
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

        public IEnumerable<T> GetNeighbours(T field)
        {
            return GetNeighbours(pointGetter(field));
        }
        public IEnumerable<T> GetNeighbours(Point p)
        {
            return p.GetNeighbours()
                .Where(Dic.ContainsKey)
                .Select(p => Dic[p]);
        }

        public T GetDef(Point coordinates)
        {
            if (Dic.TryGetValue(coordinates, out var target))
                return target;
            return default(T);
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
    public static class Extensions
    {
        public static void ToConsole<T>(this T[,] canvas, Func<T, string> tranform = null)
        {
            var innerTransform = tranform ?? new Func<T, string>((x) => x.ToString());

            Console.WriteLine($"\r\n[{canvas.GetUpperBound(0) + 1},{canvas.GetUpperBound(1) + 1}]");
            for (int y = 0; y <= canvas.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= canvas.GetUpperBound(0); x++)
                {
                    Console.Write(innerTransform(canvas[x, y]));
                }
                Console.Write("\r\n");
            }
            Console.Write("\r\n");
        }

        public static int ToInt(this char character)
        {
            return int.Parse(character.ToString());
        }
    }
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Peak<T>(this IEnumerable<T> enumerable, string prefix = "Peak: ")
        {
            return enumerable.Select(x =>
            {
                Console.WriteLine($"{prefix}{x}");
                return x;
            });
        }

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

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
    public static class StringExtensions
    {
        public static bool RegexMatch(this string line, string expr)
        {
            return Regex.IsMatch(line, expr);
        }

        public static IEnumerable<string> WhereRegexMatch(this IEnumerable<string> tests, string expr)
        {
            return tests.Where(t => t.RegexMatch(expr));
        }

        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
        public static IEnumerable<IEnumerable<string>> Splizz(this IEnumerable<string> enumerable, params string[] seps)
        {
            foreach (var item in enumerable)
            {
                yield return item.Splizz(seps);
            }
        }

        public static IEnumerable<T> RegExAutoParse<T>(this IEnumerable<string> enumerable, string pattern)
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
                    if (prop == null)
                        throw new Exception($"Property '{group}' not found on type '{type.Name}', candidates were {props.ToCommaString()}");

                    prop.SetValue(t, Convert.ChangeType(capture.Value, prop.PropertyType));
                }

                yield return t;
            }
        }
    }
}
