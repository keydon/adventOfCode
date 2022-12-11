using System;
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

        // https://stackoverflow.com/a/44203452
        public static bool isPrime(int number)
        {
            if (number == 1) return false;
            if (number == 2 || number == 3 || number == 5) return true;
            if (number % 2 == 0 || number % 3 == 0 || number % 5 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            // You can do less work by observing that at this point, all primes 
            // other than 2 and 3 leave a remainder of either 1 or 5 when divided by 6. 
            // The other possible remainders have been taken care of.
            int i = 6; // start from 6, since others below have been handled.
            while (i <= boundary)
            {
                if (number % (i + 1) == 0 || number % (i + 5) == 0)
                    return false;

                i += 6;
            }

            return true;
        }

        internal static int KgV(params int[] ints)
        {
            var kgv = ints.SelectMany(n => n.GetPrimeFactorsWithPowers())
                .GroupBy(k => k.Base, (k,v) => Math.Pow(k, v.Max(v => v.Exp)))
                .MultiplyMany(x => (int)x);
            
            ints.ToCommaString().Debug("kgV von");
            return (int)kgv.Debug("ist");
        }

        public static IEnumerable<int> GetPrimeFactors(this int n)
        {
            var i = 2;
            while (i <= n)
            {
                if (n % i == 0)
                {
                    n /= i;
                    yield return i;
                }
                else i++;
            }
        }

        public static IEnumerable<(int Base, int Exp)> GetPrimeFactorsWithPowers(this int n)
        {
            return GetPrimeFactors(n)
                .GroupBy(x => x)
                .Select(group => (Base: group.Key, Exp: group.Count()));
        }
    }
}