namespace aoc
{
    record Range
    {
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsInRange(int number) => Start <= number && number <= End;
        public bool IsOverlaping(Range other) => (Start <= other.Start && other.Start <= End) || (Start <= other.End && other.End <= End) ||
                                                 (other.Start <= Start && Start <= other.End) || (other.Start <= End && End <= other.End);
        public bool FullyContains(Range other) => (Start <= other.Start && other.Start <= End) && (Start <= other.End && other.End <= End);
    }
}