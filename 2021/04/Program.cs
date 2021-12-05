using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int Number { get; set; }
        public bool Marked { get; set; }
        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();

            var (drawnNumbers, boardsn) = LoadFoos("input.txt");

            var boards = new List<Field<Point2, Foo<Point2>>>();
            foreach (var b in boardsn)
            {
                var field = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
                field.Add(b);

                boards.Add(field);
            }

            var partOneCompleted = false;
            var bingos = new List<Field<Point2, Foo<Point2>>>();

            foreach (var number in drawnNumbers)
            {
                foreach (var board in boards)
                {
                    MarkNumber(board, number); 
                    if (IsBingo(board)){
                        if (!partOneCompleted) {
                            win(board, number).AsResult1();
                            partOneCompleted = true;
                        }
                        
                        bingos.Add(board);
                    }
                }

                boards = boards.Except(bingos).ToList();

                if (bingos.Count > 0 && boards.Count == 0){
                    win(bingos.Single(), number).AsResult2();
                }
                bingos.Clear();
            }
            
            Report.End();
        }

        private static int win(Field<Point2, Foo<Point2>> board, int winningNo)
        {
            winningNo.Debug("Winning Number");
            var sum = board.AllFields.Where(f => !f.Marked).Sum(f => f.Number).Debug("Unmarked Sum");            
            board.ToConsole(f => f.Marked ? $"({f.Number})".PadLeft(5): f.Number.ToString().PadLeft(5));
            return sum * winningNo;
        }

        private static bool IsBingo(Field<Point2, Foo<Point2>> board)
        {
            var horizontal = board.SelectEachRow(p => board.GetOrElse(p, (f) => f, null)).Any(row => row.All(c => c.Marked));
            if (horizontal)
                return true;
            var vertical = board.SelectEachCol(p => board.GetOrElse(p, (f) => f, null)).Any(row => row.All(c => c.Marked));
            if (vertical)
                return true;
            //List<IEnumerable<Foo<Point2>>> diags = board.SelectEachDiag(p => board.GetOrElse(p, (f) => f, null)).Where(row => row.All(c => c.Bingo)).ToList();
            //if(diags.Count > 0)
            //    return true;
            
            return false;
        }

        private static void MarkNumber(Field<Point2, Foo<Point2>> board, int drawnNumber)
        {
            foreach (var item in board.AllFields)
            {
              if (item.Number == drawnNumber)  {
                  item.Marked = true;
              }
            } 
        }

        public static (IEnumerable<int> bingonumbers, List<IEnumerable<Foo<Point2>>> boards) LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .ToList();

            var bingonumbers = foos.First().First().Splizz(",").Select(int.Parse);
            var boards = foos.Skip(1)
                .Select(board => board
                    .SelectMany((row, y) => row
                        .Splizz(" ")
                        .Select(int.Parse)
                        .Select((foo, x) => new Foo<Point2> { Pos = new Point2(x, y), Number = foo })
                    )
                )
                .ToList();

            return (bingonumbers, boards);
        }
    }
}
