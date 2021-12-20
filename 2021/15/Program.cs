using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Paths
    {
        public Paths(Cave start){
            Active.Add(new Path(start));
        }

        public HashSet<Path> Active {get; set;} = new HashSet<Path>();
        public HashSet<Path> Completed {get; set;} = new HashSet<Path>();
        public Path Best {get; set; } 
        private readonly Dictionary<Point2, (Path path, int score)> highScores = new();

        public void AddCompleted(Path path){
            AddActive(path);
            Active.Remove(path);
            Completed.Add(path);
            Best = Completed.OrderBy(p => p.TotalCost).First();
            Best.Debug("New Best!");
            Active.RemoveWhere(p => p.TotalCost >= Best.TotalCost);
        }

        internal bool AddActive(Path path)
        {
            if( Best != null && path.TotalCost > Best.TotalCost) {
                return false;
            }
            if (highScores.TryGetValue(path.Head.Pos, out var best)){
                if (best.score > path.TotalCost){
                    Active.Remove(best.path);
                } else {
                    return false;
                }
            }

            highScores[path.Head.Pos] = (path, path.TotalCost);
            Active.Add(path);
            return true;
        }

        public Path PopBestActive()
        {   
            var best = Active
                    .OrderBy(x => x.TotalCost)
                    .FirstOrDefault();
                    
            Active.Remove(best);
            return best;
        }
    }

    record Path {
        public Path(Cave head){
            Caves = new HashSet<Cave>();
            TotalCost = head.RiskLevel;
            Head = head;
        }
        public Path(Path path){
            Caves = new HashSet<Cave>(path.Caves);
            TotalCost = path.TotalCost;
            Head = path.Head;
        }
        public HashSet<Cave> Caves {get; set;}
        public int TotalCost {get; internal set;}
        public Cave Head { get; internal set; }

        public bool Contains(Cave cave){
            return Caves.Contains(cave);
        }
        public void Add(Cave cave){
            Caves.Add(cave);
            TotalCost += cave.RiskLevel;
            Head = cave;
        }

        public override string ToString()
        {
            return $"Distance: {Caves.Count}, Head: {Head}, Total: {TotalCost}";
        }
    }
    record Cave : IHasPosition<Point2>
    {
        public int RiskLevel { get; set; }
        public Point2 Pos { get; set; }

        public override int GetHashCode()
        {
            return Pos.GetHashCode();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var levels = LoadRiskLevels("input.txt");
            // levels = LoadRiskLevels("sample.txt");

            var field = new Field<Point2, Cave>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(levels);

            FindBestPathsTotalCost(field).AsResult1();
            
            var bigfield = BlowFieldBigger(field, 5);
            FindBestPathsTotalCost(bigfield).AsResult2();

            Report.End();
        }

        private static int FindBestPathsTotalCost(Field<Point2, Cave> field)
        {
            var start = field.Dic[new Point2(field.MinX, field.MinY)];
            var end = field.Dic[new Point2(field.MaxX, field.MaxY)];

            var bestPath = FindBestPath(field, start, end);
            return bestPath.TotalCost - start.RiskLevel;
        }

        private static Path FindBestPath(Field<Point2, Cave> field, Cave start, Cave target)
        {
            var paths = new Paths(start);
            while(true){
                var path = paths.PopBestActive();
                
                if (path == null){
                    return paths.Best;
                }
            
                var neighbours = field.GetSimpleNeighbours(path.Head);
                foreach (var neighbour in neighbours)
                {
                    var newPath = new Path(path);
                    newPath.Add(neighbour);
                    if (neighbour == target){
                        paths.AddCompleted(newPath);
                    } else {
                        paths.AddActive(newPath);
                    }
                }
            }
        }        

        private static Field<Point2, Cave> BlowFieldBigger(Field<Point2, Cave> field, int times)
        {
            var smallFieldsPoints = field.AllFields;
            var bigfield = new Field<Point2, Cave>(OutOfBoundsStrategy.RETURN_NULL);
            for (int y = 0; y < times; y++)
            {
                var baseY = field.MaxY * y + y;
                for (int x = 0; x < times; x++)
                {
                    var increase = y + x;
                    var baseX = field.MaxX * x + x;
                    bigfield.Add(
                        smallFieldsPoints.Select(f => new Cave()
                        {
                            Pos = new Point2(baseX + f.Pos.X, baseY + f.Pos.Y),
                            RiskLevel = (f.RiskLevel + increase) % 10 + (f.RiskLevel + increase) / 10
                        })
                    );
                }
            }

            return bigfield;
        }

        public static List<Cave> LoadRiskLevels(string inputTxt)
        {
            var levels = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((p, t) => new Cave { Pos = p, RiskLevel = int.Parse(t) })
                .ToList();

            return levels;
        }
    }
}
