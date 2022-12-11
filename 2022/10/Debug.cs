using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class DebugExtensions
    {
        public static string ToCommaString<T>(this IEnumerable<T> items, string delimiter = ", ")
        {
            return string.Join(delimiter, items.Select(i => i.ToString()));
        }
        public static List<T> Dump<T>(this List<T> items, object context = null)
        {
            return Debug(items, context);
        }
        public static List<T> Debug<T>(this List<T> items, object context = null)
        {
            Console.WriteLine($"DEBUG {context}: List.Count: " + (items == null ? "NULL INSTEAD OF EMPTY LIST" : items.Count));
            if(items == null)
                return items;
            var max = Math.Min(items.Count, 10);
            for (int i = 0; i < max; i++)
            {
                Console.WriteLine($"DEBUG {context} [{i}]: {items[i]}");
            }
            Console.WriteLine("DEBUG {context}: End of List.");
            return items;
        }
        public static List<List<T>> Debug<T>(this List<List<T>> items, object context = null)
        {
            Console.WriteLine($"DEBUG {context}: List.Count: " + items.Count);
            const int limit = 10;
            var max = Math.Min(items.Count, limit);
            for (int i = 0; i < max; i++)
            {
                Console.WriteLine($"DEBUG {context} [{i}]: {items[i].ToDebugString()}");

                if (limit == i)
                {
                    Console.WriteLine($"DEBUG {context} [...] !!!!!!!!!!!!!!!");
                }
            }
            Console.WriteLine("DEBUG {context}: End of List.");
            return items;
        }

        public static IEnumerable<T> Dump<T>(this IEnumerable<T> items, object context = null)
        {
            return Debug(items, context);
        }
        public static IEnumerable<T> Debug<T>(this IEnumerable<T> items, object context = null)
        {
            Console.WriteLine($"DEBUG {context} Enumerable ...Take(10): ");
            var index = 0;
            const int limit = 20;
            foreach (var item in items.Take(limit))
            {
                Console.WriteLine($"DEBUG {context} [{index++}]: {item}");
                yield return item;
                if (limit == index)
                {
                    Console.WriteLine($"DEBUG {context} [...] !!!!!!!!!!!!!!!");
                }
            }
            Console.WriteLine($"DEBUG {context} End of Enumerable.");
        }
        public static IEnumerable<IEnumerable<T>> Debug<T>(this IEnumerable<IEnumerable<T>> items, object context = null)
        {
            Console.WriteLine($"DEBUG {context} Enumerable ...Take(10): ");
            var index = 0;
            const int limit = 10;
            foreach (var item in items.Take(limit))
            {
                Console.WriteLine($"DEBUG {context} [{index++}]: {item.ToDebugString()}");
                yield return item;
                if (limit == index)
                {
                    Console.WriteLine($"DEBUG {context} [...] !!!!!!!!!!!!!!!");
                }
            }
            Console.WriteLine($"DEBUG {context} End of Enumerable.");
        }
        public static IEnumerable<List<T>> Debug<T>(this IEnumerable<List<T>> items, object context = null)
        {
            Console.WriteLine($"DEBUG {context} Enumerable ...Take(10): ");
            var index = 0;
            const int limit = 10;
            foreach (var item in items.Take(limit))
            {
                Console.WriteLine($"DEBUG {context} [{index++}]: {item.ToDebugString()}");
                yield return item;
                if (limit == index)
                {
                    Console.WriteLine($"DEBUG {context} [...] !!!!!!!!!!!!!!!");
                }
            }
            Console.WriteLine($"DEBUG {context} End of Enumerable.");
        }

        public static T Debug<T>(this T something, object context = null)
        {
            return Dump(something, context);
        }

        public static T Dump<T>(this T something, object context = null)
        {
            if (Equals(something, default(T)))
            {
                Console.WriteLine($"DEBUG {context} is NULL/default: {something}");
                return something;
            }
            Console.WriteLine($"DEBUG {context}: {something}");
            return something;
        }

        public static string ToDebugString<T>(this IEnumerable<T> items)
        {
            var list = items.ToList();
            return $"Enumberable<{typeof(T).Name}>({list.Count}): {list.ToCommaString()}";
        }
        public static string ToDebugString<T>(this List<T> items)
        {
            var list = items.ToList();
            return $"List<{typeof(T).Name}>({list.Count}): {list.ToCommaString()}";
        }

        public static IEnumerable<T> Peek<T>(this IEnumerable<T> enumerable, string prefix = "Peek: ")
        {
            return enumerable.Select(x =>
            {
                Console.WriteLine($"{prefix}{x}");
                return x;
            });
        }
    }
}