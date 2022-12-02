using System.Collections.Generic;

namespace aoc
{
    public static class Generators
    {
        public static IEnumerable<long> MultipleOf(int number)
        {
            long multiple = 0;
            while (true)
            {
                multiple += number;
                yield return multiple;
            }
        }
    }
}