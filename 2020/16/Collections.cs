using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class EnumberableCollectionsExtensions
    {
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
    }
}