using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace aoc
{
    public interface IHasPosition<P>
        where P : IPointish
    {
        P Pos { get; set; }
    }

    public interface IPointish
    {
        int X { get; set; }
        int Y { get; set; }
        IEnumerable<IPointish> GetNeighbours();
    }

    public record Point2 : IPointish
    {
        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public IEnumerable<IPointish> GetNeighbours()
        {
            for (int x = X - 1; x <= X + 1; x++)
            {
                for (int y = Y - 1; y <= Y + 1; y++)
                {
                    var p = new Point2(x, y);
                    if (p != this)
                        yield return p;
                }
            }
        }

        public static implicit operator Point2(Point p) => new Point2(p.X, p.Y);
        public static implicit operator Point(Point2 p) => new Point(p.X, p.Y);
    }

    public record Point3 : IPointish
    {
        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public IEnumerable<IPointish> GetNeighbours()
        {
            for (int x = X - 1; x <= X + 1; x++)
            {
                for (int y = Y - 1; y <= Y + 1; y++)
                {
                    for (int z = Z - 1; z <= Z + 1; z++)
                    {
                        var p = new Point3(x, y, z);
                        if (p != this)
                            yield return p;
                    }
                }
            }
        }

        public static implicit operator Point3(Point2 p) => new Point3(p.X, p.Y, 0);
    }

    public record Point4 : IPointish
    {
        public Point4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }

        public IEnumerable<IPointish> GetNeighbours()
        {
            for (int x = X - 1; x <= X + 1; x++)
            {
                for (int y = Y - 1; y <= Y + 1; y++)
                {
                    for (int z = Z - 1; z <= Z + 1; z++)
                    {
                        for (int w = W - 1; w <= W + 1; w++)
                        {
                            var p = new Point4(x, y, z, w);
                            if (p != this)
                                yield return p;
                        }
                    }
                }
            }
        }
        public static implicit operator Point4(Point2 p) => new Point4(p.X, p.Y, 0, 0);
    }

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

    public static class Point2Extensions
    {
        public static Point2 Left(this Point2 p, int step = 1) => new Point2(p.X - step, p.Y);
        public static Point2 Up(this Point2 p, int step = 1) => new Point2(p.X, p.Y - step);
        public static Point2 Right(this Point2 p, int step = 1) => new Point2(p.X + step, p.Y);
        public static Point2 Down(this Point2 p, int step = 1) => new Point2(p.X, p.Y + step);

        public static Point2 Move(this Point2 p, Point2 direction, int step = 1) => new Point2(p.X + (direction.X * step), p.Y + (direction.Y * step));
        public static Point2 Move(this Point2 p, Direction direction, int step = 1) => direction switch
        {
            Direction.UP => p.Up(step),
            Direction.RIGHT => p.Right(step),
            Direction.LEFT => p.Left(step),
            Direction.DOWN => p.Down(step),
        };

        public static Point2 RotateRight(this Point2 p, Point2 rel = default) => new Point2(((p.Y - rel.Y) * -1) + rel.X, (p.X - rel.X) + rel.Y);
        public static Point2 RotateLeft(this Point2 p, Point2 rel = default) => new Point2((p.Y - rel.Y) + rel.X, ((p.X - rel.X) * -1) + rel.Y);

        public static Point2 Rotate180(this Point2 p, Point2 rel = default) => new Point2(((p.X - rel.X) * -1) + rel.X, ((p.Y - rel.Y) * -1) + rel.Y);

        public static Point2 Rotate(this Point2 p, Rotation r, Point2 rel = default)
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

        public static IEnumerable<Point2> GetNeighbours(this Point2 p)
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

        public static int ManhattenDistance(this Point2 Point2, Point2 other = default)
        {
            return Math.Abs(Point2.X - other.X) + Math.Abs(Point2.Y - other.Y);
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

    public static class VectorExtensions
    {
        public static Vector3 Left(this Vector3 p, int step = 1) => new Vector3(p.X - step, p.Y, p.Z);
        public static Vector3 Up(this Vector3 p, int step = 1) => new Vector3(p.X, p.Y - step, p.Z);
        public static Vector3 Right(this Vector3 p, int step = 1) => new Vector3(p.X + step, p.Y, p.Z);
        public static Vector3 Down(this Vector3 p, int step = 1) => new Vector3(p.X, p.Y + step, p.Z);

        public static Vector3 Deeper(this Vector3 p, int step = 1) => new Vector3(p.X, p.Y, p.Z + step);
        public static Vector3 Narrower(this Vector3 p, int step = 1) => new Vector3(p.X, p.Y, p.Z - step);


        public static Vector3 Move(this Vector3 p, Vector3 direction, int step = 1) => new Vector3(p.X + (direction.X * step), p.Y + (direction.Y * step), 0);
        public static Vector3 Move(this Vector3 p, Direction direction, int step = 1) => direction switch
        {
            Direction.UP => p.Up(step),
            Direction.RIGHT => p.Right(step),
            Direction.LEFT => p.Left(step),
            Direction.DOWN => p.Down(step),
        };

        public static IEnumerable<Vector3> GetNeighbours(this Vector3 p)
        {
            yield return p.Up();
            yield return p.Left();
            yield return p.Right();
            yield return p.Down();
            yield return p.Up().Left();
            yield return p.Up().Right();
            yield return p.Down().Left();
            yield return p.Down().Right();

            yield return p.Deeper();
            yield return p.Deeper().Up();
            yield return p.Deeper().Left();
            yield return p.Deeper().Right();
            yield return p.Deeper().Down();
            yield return p.Deeper().Up().Left();
            yield return p.Deeper().Up().Right();
            yield return p.Deeper().Down().Left();
            yield return p.Deeper().Down().Right();

            yield return p.Narrower();
            yield return p.Narrower().Up();
            yield return p.Narrower().Left();
            yield return p.Narrower().Right();
            yield return p.Narrower().Down();
            yield return p.Narrower().Up().Left();
            yield return p.Narrower().Up().Right();
            yield return p.Narrower().Down().Left();
            yield return p.Narrower().Down().Right();
        }

        public static int ManhattenDistance(this Vector3 point, Vector3 other = default)
        {
            return Math.Abs((int)point.X - (int)other.X) + Math.Abs((int)point.Y - (int)other.Y);
        }
    }

}