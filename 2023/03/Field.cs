using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace aoc
{
    public enum OutOfBoundsStrategy
    {
        RETURN_NULL,
        CREATE_NEW
    }

    public record SimpleDrawable : IHasPosition<Point2>
    {
        public string DisplayName {get; set; }
        public Point2 Pos { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class Field<P, T>
        where T : IHasPosition<P>, new()
        where P : IPointish
    {

        public string EmptyField { get; set; } = ".";
        public int MaxY { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MinX { get; private set; }
        public OutOfBoundsStrategy OutOfBoundsStrategy { get; set; }

        public readonly Dictionary<IPointish, T> Dic = new();
        public readonly LinkedList<T> AllFields = new();

        public R GetOrElse<R>(IPointish p, Func<T, R> extractor, Func<IPointish, R> elze)
        {
            if (Dic.TryGetValue(p, out var v))
            {
                return extractor(v);
            }
            return elze(p);
        }

        public T GetNew(IPointish p, Action<T> initNew = null)
        {
            if (Dic.TryGetValue(p, out var v))
            {
                return v;
            }
            var foo = new T
            {
                Pos = (P)p
            };
            initNew?.Invoke(foo);

            Add(foo);
            return foo;
        }
        public Field(OutOfBoundsStrategy outOfBoundsStrategy)
        {
            OutOfBoundsStrategy = outOfBoundsStrategy;
        }

        public void Add(T item)
        {
            var p = item.Pos;
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

        internal void Remove(Point2 p)
        {
            if (Dic.TryGetValue(p, out var val)){
                AllFields.Remove(val);
                Dic.Remove(p);

                MaxX = AllFields.Max(f => f.Pos.X);
                MaxY = AllFields.Max(f => f.Pos.Y);
                MinX = AllFields.Min(f => f.Pos.X);
                MinY = AllFields.Min(f => f.Pos.Y);
            }
        }

        internal void BulkTrim(List<Point2> border)
        {
            foreach (var p in border)
            {
                if (Dic.TryGetValue(p, out var val)){
                    AllFields.Remove(val);
                    Dic.Remove(p);
                }   
            }
             
                MaxX = AllFields.Max(f => f.Pos.X);
                MaxY = AllFields.Max(f => f.Pos.Y);
                MinX = AllFields.Min(f => f.Pos.X);
                MinY = AllFields.Min(f => f.Pos.Y);
        }

        public IEnumerable<T> GetNeighbours(T foo, Action<T> initNew = null)
        {
            return GetNeighbours(foo.Pos, initNew);
        }
        public IEnumerable<T> GetNeighbours(P p, Action<T> initNew = null)
        {
            return OutOfBoundsStrategy == OutOfBoundsStrategy.CREATE_NEW
                ? p.GetNeighbours().Select(p => GetNew(p, initNew))
                : p.GetNeighbours().Where(Dic.ContainsKey).Select(Dic.GetValueOrDefault);
        }
        public IEnumerable<T> GetSimpleNeighbours(T foo, Action<T> initNew = null)
        {
            return GetSimpleNeighbours(foo.Pos, initNew);
        }
        public IEnumerable<T> GetSimpleNeighbours(P p, Action<T> initNew = null)
        {
            return OutOfBoundsStrategy == OutOfBoundsStrategy.CREATE_NEW
                ? p.GetSimpleNeighbours().Select(p => GetNew(p, initNew))
                : p.GetSimpleNeighbours().Where(Dic.ContainsKey).Select(Dic.GetValueOrDefault);
        }

    }

    public static class FieldExtensions
    {
        public static void ToConsole<T>(this Field<Point2, T> f, Func<T, string> printer)
        where T : IHasPosition<Point2>, new()
        {
            var lines = f.SelectEachRow(p => f.GetOrElse<string>(p, printer, p => f.EmptyField))
                .Select(row => row.ToCommaString(""));
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public static IEnumerable<IEnumerable<R>> SelectEachRow<R, T>(this Field<Point2, T> f, Func<Point2, R> extractor)
            where T : IHasPosition<Point2>, new()
        {
            //Console.WriteLine($"[{f.MinX},{f.MinY}]x[{f.MaxX},{f.MaxY}]");
            return Enumerable.Range(f.MinY, f.MaxY - f.MinY + 1)
                .Select(y =>
                    Enumerable.Range(f.MinX, f.MaxX - f.MinX + 1)
                        .Select(x => new Point2(x, y))
                        .Select(extractor));
        }
        public static IEnumerable<IEnumerable<R>> SelectEachCol<R, T>(this Field<Point2, T> f, Func<Point2, R> extractor)
            where T : IHasPosition<Point2>, new()
        {
            //Console.WriteLine($"[{f.MinX},{f.MinY}]x[{f.MaxX},{f.MaxY}]");
            return Enumerable.Range(f.MinY, f.MaxY - f.MinY + 1)
                .Select(y =>
                    Enumerable.Range(f.MinX, f.MaxX - f.MinX + 1)
                        .Select(x => new Point2(y, x))
                        .Select(extractor));
        }

        
        public static IEnumerable<IEnumerable<R>> SelectEachDiag<R, T>(this Field<Point2, T> f, Func<Point2, R> extractor)
            where T : IHasPosition<Point2>, new()
        {
            //Console.WriteLine($"[{f.MinX},{f.MinY}]x[{f.MaxX},{f.MaxY}]");
            var one = Enumerable.Range(f.MinY, f.MaxY+1)
                        .Select(x => new Point2(x, x))
                        .Select(extractor);

                        var diag = new List<Point2>(){
                            new Point2(4,0),
                            new Point2(3,1),
                            new Point2(2,2),new Point2(1,3),new Point2(0,4),
                        };
                        var two = diag.Select(extractor);

                        yield return one;
                        yield return two;
        }

        public static void ToConsole(this IEnumerable<Point2> points, Func<Point2, string> printer = null){
            Func<Point2, string> defaultPrinter = (p) => Symbols.Full;
            var print = printer ?? defaultPrinter;

            var field = new Field<Point2, SimpleDrawable>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(points.Select(p => new SimpleDrawable(){Pos = p}));
            field.ToConsole(f => print.Invoke(f.Pos));
        }
    }

}