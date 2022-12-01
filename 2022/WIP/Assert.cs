using System;

namespace aoc
{
    public static class TestingExtensions
    {
        public static T Assert<T>(this T something, T expected, object context = null)
        {
            if (Equals(something, default))
            {
                Console.WriteLine($"ERROR {context} is NULL/default: {something}");
                return something;
            }
            if (Equals(something, expected))
            {
                Console.WriteLine($"OK {something}");
                return something;
            }
            Console.WriteLine($"ERROR Expected: {expected} Actual: {something}");
            return something;
        }
    }
}