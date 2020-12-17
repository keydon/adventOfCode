using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {

        public string CurrentState { get; set; }
        public string NextState { get; set; }

        public TPoint Pos { get; set; }

        public void Apply()
        {
            CurrentState = NextState;
            NextState = " ";
        }

        public bool IsAlive => CurrentState == "#";

        public void Next(Field<TPoint, Foo<TPoint>> field)
        {
            var aliveNeighbours = field.GetNeighbours(this).Count(f => f.IsAlive);
            NextState = (CurrentState, aliveNeighbours) switch
            {
                ("#", 2) => "#",
                (_, 3) => "#",
                (_, _) => "."
            };
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            Play(foos, 6, p => (Point3)p).AsResult1();
            Play(foos, 6, p => (Point4)p).AsResult2();

            Report.End();
        }

        private static int Play<TPoint>(List<Foo<Point2>> foos, int cycles, Func<Point2, TPoint> pointConverter)
            where TPoint : IPointish
        {
            var field = new Field<TPoint, Foo<TPoint>>(OutOfBoundsStrategy.CREATE_NEW);
            field.Add(foos.Select(f => new Foo<TPoint>() { CurrentState = f.CurrentState, Pos = pointConverter(f.Pos) }));

            for (int i = 1; i <= cycles; i++)
            {
                i.Debug("cycle");
                field.OutOfBoundsStrategy = OutOfBoundsStrategy.CREATE_NEW;
                var count = field.AllFields
                   .Where(f => f.IsAlive)
                   .ToList()
                   .SelectMany(f => field.GetNeighbours(f))
                   .Count();
                count.Debug("grown outer layer");

                field.OutOfBoundsStrategy = OutOfBoundsStrategy.RETURN_NULL;
                foreach (var f in field.AllFields)
                {
                    f.Next(field);
                }
                foreach (var f in field.AllFields)
                {
                    f.Apply();
                }
            }
            return field.AllFields.Count(f => f.IsAlive);
        }

        public static List<Foo<Point2>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany((row, y) => row.Select((foo, x) => new Foo<Point2> { Pos = new Point2(x, y), CurrentState = foo.ToString() }))
                .ToList();

            return foos;
        }
    }
}
