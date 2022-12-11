using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Tree<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int Height { get; set; }
        public TPoint Pos { get; set; }
    }

    class ViewLine {
        public HashSet<Point2> VisiblePoints {get; set;} = new HashSet<Point2>();
        public int Heighest = -1;
        public Boolean done = false;
        private readonly int ViewPointHeight;

        public ViewLine(int viewPointHeight)
        {
            this.ViewPointHeight = viewPointHeight;
        }

        public ViewLine DetermineVisibilityPartOne(Tree<Point2> foo){
            if (foo.Height > Heighest){
                Heighest = foo.Height;
                VisiblePoints.Add(foo.Pos);
            }
            return this;
        }

        public ViewLine DetermineVisibilityPartTwo(Tree<Point2> foo){
            if (done)
                return this;
            
            Heighest = foo.Height;
            VisiblePoints.Add(foo.Pos);    
            
            if (foo.Height >= ViewPointHeight)
                done = true;
            return this;
        }

        public override string ToString()
        {
            return Heighest + " " + VisiblePoints.ToCommaString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var treeMap = LoadTrees("input.txt");
            //treeMap = LoadTrees("sample.txt");

            var field = new Field<Point2, Tree<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            treeMap.ForEach(p => field.Add(p));
            // field.ToConsole(p => p.Height.ToString());

            var edge1 = field.SelectEachRow(tree => field.Dic[tree]).ToList();
            var edge2 = edge1.Select(trees => trees.Reverse()).ToList();
            var edge3 = field.SelectEachCol(tree => field.Dic[tree]).ToList();
            var edge4 = edge3.Select(trees => trees.Reverse()).ToList();
            
            var edges = new List<IEnumerable<IEnumerable<Tree<Point2>>>>() { edge1, edge2, edge3, edge4 };
            edges
                .SelectMany(edge => edge)
                .Select(trees => trees.Aggregate(new ViewLine(-1), (acc, tree) => acc.DetermineVisibilityPartOne(tree)))
                .SelectMany(acc => acc.VisiblePoints)
                .Distinct().Count().AsResult1();

            field.AllFields.Select(fo => 
                    fo.SelectAllDirections()
                    .Select(d => field.Walk(d.pointish.Pos, d.direction)
                        .Aggregate(new ViewLine(field.Dic[d.pointish.Pos].Height), (acc, fo) => acc.DetermineVisibilityPartTwo(fo))
                        .VisiblePoints.Count
                    )
                    .Where(x => x > 0)
                    .MultiplyAll()
                )
                .Max()
                .AsResult2();
            Report.End();
        }

        public static List<Tree<Point2>> LoadTrees(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((p, t) => new Tree<Point2> { Pos = p, Height = int.Parse(t) })
                .ToList();
        }
    }
}
