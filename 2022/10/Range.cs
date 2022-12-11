namespace aoc
{
    record Range
    {
        public int Start { get; set; }
        public int End { get; set; }
        public bool IsInRange(int number) => Start <= number && number <= End;
    }
}