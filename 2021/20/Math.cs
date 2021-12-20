using System.Collections.Generic;
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

        public static (int rest, int a, int b) Euklid(int a, int b)
        {
            if (b == 0)
                return (a, 1, 0);
            (var g, var u, var v) = Euklid(b, a % b);
            var q = a / b;
            return (g, v, u - (q * v));
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}