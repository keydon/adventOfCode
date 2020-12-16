using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class StringParserExtensions
    {
        public static IEnumerable<int> ParseInts(this string numbers, params string[] seps)
        {
            return numbers.Splizz(seps).Select(int.Parse);
        }
        public static IEnumerable<int> ParseLongs(this string numbers, params string[] seps)
        {
            return numbers.Splizz(seps).Select(int.Parse);
        }

        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
        public static IEnumerable<IEnumerable<string>> Splizz(this IEnumerable<string> enumerable, params string[] seps)
        {
            foreach (var item in enumerable)
            {
                yield return item.Splizz(seps);
            }
        }
    }
}