using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record FoldingInstruction
    {
        public string Axis { get; set; }
        public int Pos { get; set; }
    }

    record Dot : IHasPosition<Point2>
    {
        public Point2 Pos { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
           
            var (dots, foldInstructions) = LoadInput("input.txt");
            // (dots, foldInstructions) = LoadInput("sample.txt" +"");

            var field = new Field<Point2, Dot>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(dots);

            var firstFold = true;
            foreach (var fold in foldInstructions)
            {
                var (direction, selectAxisPos) = fold.Axis == "y" 
                    ? (new Point2(0, -1), (Func<Dot, int>)((dot) => dot.Pos.Y)) 
                    : (new Point2(-1, 0), (Func<Dot, int>)((dot) => dot.Pos.X));

                var toFold = field.AllFields.Where(dot => selectAxisPos(dot) > fold.Pos).ToList();                
                foreach (var dot in toFold)
                {
                    field.Dic.Remove(dot.Pos);
                    var distance = 2 * Math.Abs(selectAxisPos(dot) - fold.Pos);
                        
                    dot.Pos = dot.Pos.Move(direction, distance);

                    if(!field.Dic.ContainsKey(dot.Pos)) {
                        field.Dic.Add(dot.Pos, dot);
                    } else {
                        field.AllFields.Remove(dot);
                    }
                }
                
                if (firstFold){
                    firstFold = false;
                    field.AllFields.Count.AsResult1();
                }
            }

            var part2 = new Field<Point2, Dot>(OutOfBoundsStrategy.RETURN_NULL);
            part2.EmptyField = " ";
            part2.Add(field.AllFields);
            part2.ToConsole(f => "#");

            Report.End();
        }

        public static (List<Dot> dots, List<FoldingInstruction> foldInstructions) LoadInput(string inputTxt)
        {
            var parts = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator();

            var dots = parts.First()
                .Select(s => s.ParseRegex(@"^(\d+),(\d+)$", m => new Dot()
                {
                   Pos = new Point2(
                       int.Parse(m.Groups[1].Value),
                       int.Parse(m.Groups[2].Value))
                }))
                .ToList();

            var foldInstructions = parts.Last()
                .Select(s => s.ParseRegex(@"^fold along (.)=(\d+)$", m => new FoldingInstruction()
                {
                   Axis = m.Groups[1].Value,
                   Pos = int.Parse(m.Groups[2].Value),
                }))
                .ToList();

            return (dots, foldInstructions);
        }
    }
}
