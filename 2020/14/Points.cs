using System;
using System.Collections.Generic;
using System.Drawing;

namespace aoc
{
    public enum Rotation
    {
        LEFT, RIGHT, TURNAROUND, STAY
    }
    public enum Direction
    {
        UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3
    }
    public static class PointParseUtils
    {
        public static Rotation ParseRotation(string direction, int degrees = 0) => (direction, degrees) switch
        {
            (_, 180) => Rotation.TURNAROUND,
            ("R", 270) => Rotation.LEFT,
            ("L", 270) => Rotation.RIGHT,
            ("R", _) => Rotation.RIGHT,
            ("L", _) => Rotation.LEFT,
        };
        public static Direction ParseDirection(string direction)
        {
            return direction switch
            {
                "N" or "UP" => Direction.UP,
                "E" or "RIGHT" or "R" or "O" => Direction.RIGHT,
                "W" or "LEFT" or "L" => Direction.LEFT,
                "S" or "DOWN" or "D" => Direction.DOWN,
                _ => throw new Exception("Unknonw direction: " + direction),
            };
        }
    }

    public static class DirectionExtensions
    {
        public static Direction RotateRight(this Direction p) => p.Rotate(Rotation.RIGHT);
        public static Direction RotateLeft(this Direction p) => p.Rotate(Rotation.LEFT);
        public static Direction Rotate180(this Direction p) => p.Rotate(Rotation.TURNAROUND);

        public static Direction Rotate(this Direction direction, Rotation r)
        {
            var currentDirection = (int)direction;
            var rotation = r switch
            {
                Rotation.LEFT => -1,
                Rotation.RIGHT => +1,
                Rotation.TURNAROUND => +2,
                Rotation.STAY => 0,
                _ => throw new Exception("Unknown rotation" + r),
            };
            var newDirection = (currentDirection + rotation) % 4;
            if (newDirection < 0)
            {
                newDirection = 4 + newDirection;
            }
            return (Direction)newDirection;
        }
    }

    public static class PointExtensions
    {
        public static Point Left(this Point p, int step = 1) => new Point(p.X - step, p.Y);
        public static Point Up(this Point p, int step = 1) => new Point(p.X, p.Y - step);
        public static Point Right(this Point p, int step = 1) => new Point(p.X + step, p.Y);
        public static Point Down(this Point p, int step = 1) => new Point(p.X, p.Y + step);

        public static Point Move(this Point p, Point direction, int step = 1) => new Point(p.X + (direction.X * step), p.Y + (direction.Y * step));
        public static Point Move(this Point p, Direction direction, int step = 1) => direction switch
        {
            Direction.UP => p.Up(step),
            Direction.RIGHT => p.Right(step),
            Direction.LEFT => p.Left(step),
            Direction.DOWN => p.Down(step),
        };

        public static Point RotateRight(this Point p, Point rel = default) => new Point(((p.Y - rel.Y) * -1) + rel.X, (p.X - rel.X) + rel.Y);
        public static Point RotateLeft(this Point p, Point rel = default) => new Point((p.Y - rel.Y) + rel.X, ((p.X - rel.X) * -1) + rel.Y);

        public static Point Rotate180(this Point p, Point rel = default) => new Point(((p.X - rel.X) * -1) + rel.X, ((p.Y - rel.Y) * -1) + rel.Y);

        public static Point Rotate(this Point p, Rotation r, Point rel = default)
        {
            return r switch
            {
                Rotation.LEFT => p.RotateLeft(rel),
                Rotation.RIGHT => p.RotateRight(rel),
                Rotation.TURNAROUND => p.Rotate180(rel),
                Rotation.STAY => p,
                _ => throw new Exception("Unknown rotation" + r),
            };
        }

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

        public static int ManhattenDistance(this Point point, Point other = default)
        {
            return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
        }
    }

}