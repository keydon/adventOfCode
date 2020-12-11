using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class DebugExtensions
    {
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

    }
}