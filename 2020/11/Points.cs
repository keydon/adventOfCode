using System;
using System.Collections.Generic;
using System.Drawing;

namespace aoc
{
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
            yield return p.Up().Left();
            yield return p.Up().Right();
            yield return p.Down().Left();
            yield return p.Down().Right();
        }
    }

}