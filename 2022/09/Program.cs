using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record SnakeElement
    {
        public string Name { get; set; }
        public Point2 Pos { get; set; }

        public SnakeElement MoveTowards(SnakeElement target){
            if (Pos.GetNeighbours().Contains(target.Pos))
                return this;
            Pos = Pos.MoveTowards(target.Pos);
            return this;
        }
    }

    class Snake{
        readonly List<SnakeElement> elements;
        readonly HashSet<Point2> visitedByTail = new();

        public SnakeElement Head => elements.First();
        public SnakeElement Tail => elements.Last();
        public int VisitedByTailCount => visitedByTail.Count;

        public Snake(int length, Point2 start){
            elements = Enumerable.Range(0, length)
                .Select(x => new SnakeElement(){
                    Pos = start,
                    Name = x.ToString()
                })
                .ToList();
            
            visitedByTail.Add(Tail.Pos);
            elements.Select(e => e.Name).ToCommaString().Debug("Snake");
        }

        public void Move(Direction direction, int steps){
            for (int s = 0; s < steps; s++)
            {
                Head.Pos = Head.Pos.Move(direction);
                for (int i = 1; i < elements.Count; i++)
                {
                    var previous = elements[i-1];
                    var current = elements[i];
                    current.MoveTowards(previous);
                }
                visitedByTail.Add(Tail.Pos);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var moves = LoadMovements("input.txt");
            //moves = LoadMovements("sample.txt");
            var snake1 = new Snake(2, new Point2(0, 0));
            var snake2 = new Snake(10, new Point2(0, 0));

            moves.ForEach(m => snake1.Move(m.Direction, m.Steps));
            moves.ForEach(m => snake2.Move(m.Direction, m.Steps));

            snake1.VisitedByTailCount.AsResult1();
            snake2.VisitedByTailCount.AsResult2();

            Report.End();
        }

        public static List<(Direction Direction, int Steps)> LoadMovements(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^([A-Z]+) (\d+)$", m => 
                (
                    Direction: PointParseUtils.ParseDirection(m.Groups[1].Value),
                    Steps: int.Parse(m.Groups[2].Value)
                )))
                .ToList();
        }
    }
}
