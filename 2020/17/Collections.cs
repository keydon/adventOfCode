using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class EnumberableCollectionsExtensions
    {
        public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> enumberable)
        {
            var items = enumberable.ToList();
            var allItems = enumberable.SelectMany(i => i).ToList();

            return items.Aggregate(allItems, (intersect, next) => intersect.Intersect(next).ToList());
        }
        public static IEnumerable<T> UnionMany<T>(this IEnumerable<IEnumerable<T>> enumberable)
        {
            return enumberable.Aggregate(new List<T>(), (union, next) => union.Union(next).ToList());
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
        /*
          var result2 = groups
                .Select(g => new
                {
                    allAnswers = g.SelectMany(a => a),
                    Group = g
                })
                .Select(g => g.Group.Aggregate(g.allAnswers, (unanimous, answers) => unanimous.Intersect(answers)))
                .Sum(g => g.Count());
        */
    }
}