using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class EnumberableCollectionsExtensions
    {
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> enumerable)
        {
            return new LinkedList<T>(enumerable);
        }
    }
}