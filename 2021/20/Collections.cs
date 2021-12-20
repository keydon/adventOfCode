using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class EnumberableCollectionsExtensions
    {
        public static long MultiplyAll(this IEnumerable<int> numbers)
        {
            return numbers.Select(n => (long)n).MultiplyAll();
        }
        public static long MultiplyAll(this IEnumerable<long> numbers)
        {
            return numbers.Aggregate((a, b) => a * b);
        }
        public static long MultiplyMany<T>(this IEnumerable<T> items, Func<T, long> longSelector)
        {
            return items.Select(longSelector).MultiplyAll();
        }

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

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize == 0)
                throw new ArgumentNullException();

            var enumer = source.GetEnumerator();
            while (enumer.MoveNext())
            {
                yield return Take(enumer.Current, enumer, chunkSize);
            }
        }

        private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int chunkSize)
        {
            while (true)
            {
                yield return head;
                if (--chunkSize == 0)
                    break;
                if (tail.MoveNext())
                    head = tail.Current;
                else
                    break;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action){
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}