using System;
using System.Collections.Generic;

namespace aoc
{
    record Range
    {
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsInRange(int number) => Start <= number && number <= End;
    }

    record LongRange
    {
        public long Start { get; set; }
        public long End { get; set; }
        public bool IsInRange(long number) => Start <= number && number <= End;

        public bool DoesIntersect(LongRange o){
            return (End >= o.Start && End <= o.End)
              || (Start >= o.Start && Start <= o.End)
              || (o.Start >= Start && o.Start <= End)
              || (o.End >= Start && o.End <= End);
        }

        internal LongRange Intersect(LongRange o)
        {
            var start = (Start >= o.Start && Start <= o.End)
                ? Start : o.Start;
            var end = (End >= o.Start && End <= o.End)
                ? End : o.End;

            return new LongRange {
                Start = start,
                End = end
            };
        }

        public static IEnumerable<long> Generate(long start, long count){
            for (long i = start; i < count; i++)
            {
                yield return i;
            }
        }
    }
}