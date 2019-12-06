using System;
using System.Drawing;

public static class PointExtensions{

    private static int CalcManhattenDistance(this Point point, Point other)
    {
            return Math.Abs(point.X- other.X) + Math.Abs(point.Y - other.Y);
    }
}