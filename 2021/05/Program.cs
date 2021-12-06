using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public TPoint Pos { get; set; }

        public HashSet<Line> Lines { get; set; } = new HashSet<Line>();

    }
    record Line
    {
        public Point2 Start { get; set; }
        public Point2 End { get; set; }

        public bool IsDiagonal(){
            if(Start.X == End.X)
                return false;
            if(Start.Y == End.Y)
                return false;
            return true;
        }

        public IEnumerable<Point2> GetPoints(){
            if(Start.X == End.X)
                return GetVerticalPoints();
            if(Start.Y == End.Y)
                return GetHorizontalpoints();
            return GetDiagonalPoints();
        }

        private IEnumerable<Point2> GetVerticalPoints()
        {
            var x = Start.X;
            var (startY, endY) = (Start.Y > End.Y) ? (End.Y, Start.Y) : (Start.Y , End.Y);
            return Enumerable.Range(startY, (endY - startY + 1))
                .Select(y => new Point2(x,y));
        }
        
        private IEnumerable<Point2> GetHorizontalpoints()
        {
            var y = Start.Y;
            var (startX, endX) = (Start.X > End.X) ? (End.X, Start.X) : (Start.X , End.X);
            return Enumerable.Range(startX, endX - startX + 1)
                .Select(x => new Point2(x,y));
        }  
        private IEnumerable<Point2> GetDiagonalPoints()
        {
            var directionY = (Start.Y > End.Y) ? -1 : +1;
            var directionX = (Start.X > End.X) ? -1 : +1;
            var direction = new Point(directionX, directionY);

            for (int i = 0; i < Math.Abs(End.X - Start.X) + 1; i++)
            {
                yield return ((Point)Start).Move(direction, i);
            }
        
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var lines = LoadLines("input.txt");

            var field = new Field<Point2,Foo<Point2>>(OutOfBoundsStrategy.CREATE_NEW);
            
            
            foreach (var line in lines)
            {
                foreach (var point in line.GetPoints())
                {
                    var tile = field.GetNew(point);
                    tile.Lines.Add(line);
                }
            }

            //field.ToConsole(f => f.Lines.Count.ToString());

            field.AllFields.Where(f => f.Lines.Where(l => !l.IsDiagonal()).Count() >= 2).Count().AsResult1();
            field.AllFields.Where(f => f.Lines.Count >= 2).Count().AsResult2();

            Report.End();
        }

        public static List<Line> LoadLines(string inputTxt)
        {
            var lines = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^(\d+),(\d+) -> (\d+),(\d+)$", m => new Line()
                {
                   Start = new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
                   End = new Point(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value)),
                }))
                .ToList();

            return lines;
        }
    }
}
