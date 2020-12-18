using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc
{
    public static class ParseExtensions
    {

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