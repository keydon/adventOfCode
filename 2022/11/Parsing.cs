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
        public static IEnumerable<int> ParseInts(this string numbers, params string[] seps)
        {
            return numbers.Splizz(seps).Select(int.Parse);
        }
        public static IEnumerable<long> ParseLongs(this string numbers, params string[] seps)
        {
            return numbers.Splizz(seps).Select(long.Parse);
        }
        
        public static IEnumerable<ulong> ParseULongs(this string numbers, params string[] seps)
        {
            return numbers.Splizz(seps).Select(ulong.Parse);
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

        public static int ToInt(this char character)
        {
            return int.Parse(character.ToString());
        }
        public static IEnumerable<T> Parse2DMap<T>(this IEnumerable<string> rawMap, Func<Point2, string, T> tileCreator)
        {
            return rawMap.SelectMany((row, y) => row.Select((foo, x) => tileCreator(new Point2(x, y), foo.ToString())));
        }

        public static IEnumerable<List<string>> GroupByLineSeperator(this IEnumerable<string> enumerable, string lineSeperator = "")
        {
            return enumerable.GroupByLineSeperator(g => g.ToList(), lineSeperator);
        }
        public static IEnumerable<T> GroupByLineSeperator<T>(this IEnumerable<string> enumerable, Func<IEnumerable<string>, T> groupSelector, string lineSeperator = "")

        {
            var group = new List<string>();
            foreach (var item in enumerable)
            {
                if (Equals(item, lineSeperator))
                {
                    if (group.Count > 0)
                    {
                        yield return groupSelector(group);
                        group = new List<string>();
                    }
                }
                else
                {
                    group.Add(item);
                }
            }
            if (group.Count > 0)
            {
                yield return groupSelector(group);
            }
        }

        public static IEnumerable<T> GroupByRegex<T>(IEnumerable<string> enumerable, string pattern, Func<List<string>, T> groupSelector)

        {
            var group = new List<string>();
            foreach (var item in enumerable)
            {
                if (item.RegexMatch(pattern))
                {
                    if (group.Count > 0)
                    {
                        yield return groupSelector(group);
                        group = new List<string>();
                    }
                }
                group.Add(item);
            }
            if (group.Count > 0)
            {
                yield return groupSelector(group);
            }
        }

        public static bool RegexMatch(this string line, string expr)
        {
            return Regex.IsMatch(line, expr);
        }

        public static IEnumerable<string> WhereRegexMatch(this IEnumerable<string> tests, string expr)
        {
            return tests.Where(t => t.RegexMatch(expr));
        }

        public static T ParseRegex<T>(this string line, string pattern, Func<Match, T> matchExtractor)
        {
            // 
            //
            // 
            Regex operationRegEx = new Regex(pattern);
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);

            return matchExtractor(match);
        }

        public static IEnumerable<T> RegExAutoParse<T>(this IEnumerable<string> enumerable, string pattern)
           where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).ToArray();
            var regex = new Regex(pattern, RegexOptions.Compiled);
            foreach (var item in enumerable)
            {
                Match match = regex.Match(item);
                if (!match.Success)
                {
                    throw new Exception($"{pattern} did not match '{item}'");
                }
                var t = new T();
                foreach (var group in match.Groups.Keys.Where(k => !int.TryParse(k, out var _)))
                {
                    var capture = match.Groups[group];
                    var prop = props.FirstOrDefault(p => string.Equals(p.Name, group, StringComparison.OrdinalIgnoreCase));
                    if (prop == null)
                        throw new Exception($"Property '{group}' not found on type '{type.Name}', candidates were {props.ToCommaString()}");

                    prop.SetValue(t, Convert.ChangeType(capture.Value, prop.PropertyType));
                }

                yield return t;
            }
        }
    }
}