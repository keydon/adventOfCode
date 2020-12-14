using System;
using System.Linq;

namespace aoc
{
    public static class AocMath
    {
        public static int GgT(params int[] numbers)
        {
            return numbers.Aggregate((a, b) =>
            {
                var aa = a > b ? a : b;
                var bb = a > b ? b : a;
                var ggt0 = -1;
                var ggt = aa % bb;
                while (ggt != 0)
                {
                    ggt0 = ggt;
                    aa = bb;
                    bb = ggt;
                    ggt = aa % bb;

                }
                return ggt0;
            });
        }

        public static (long rest, long a, long b) Euklid(long a, long b)
        {
            if (b == 0L)
                return (a, 1, 0);
            (var g, var u, var v) = Euklid(b, a % b);
            var q = a / b;
            return (g, v, u - (q * v));
        }

        public static long mod(long x, long m)
        {
            if (m < 0) m = -m;
            return (x % m + m) % m;
        }
    }
    public class ExtendedEuklid
    {
        public ExtendedEuklid()
        {
        }
        public (int g, int u, int v) ExtendetEuklid(int a, int b)
        {
            int q, r, s, t;
            int u, v, g;
            u = t = 1;
            v = s = 0;
            while (b > 0)
            {
                q = a / b;
                r = a - q * b; a = b; b = r;
                r = u - q * s; u = s; s = r;
                r = v - q * t; v = t; t = r;
                r.Debug(r);
            }
            g = a;
            return (g, u, v);
        }
    }

}