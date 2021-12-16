using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Path
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
    record Cave{
        public Cave(string id){
            Id = id;
            IsBigCave = id.ToUpper() == id;
        }
        public string Id { get; set; }        
        public bool IsBigCave { get; private set; }
        public List<Cave> Neighbours { get; set; }
        public Func<Dictionary<Cave, int>, bool> SmallCaveVisitStrategy { get; set; } = (_) => true;

        public bool CanVisit(Dictionary<Cave, int> visited)
        {
            if (IsBigCave)
                return true;
            
            if (!visited.ContainsKey(this))
                return true;
            
            if (SmallCaveVisitStrategy(visited))
                return false;
                
            return true;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var paths = LoadPaths("input.txt");

            var caves = paths
                .SelectMany(p => new[] { p.Start, p.End })
                .Distinct()
                .Select(caveId => new Cave(caveId))
                .ToList();
            
            var map = ConnectCaves(paths, caves);
            var start = map["start"];
            
            VisitAll(start).Count().AsResult1();

            caves.ForEach(c => c.SmallCaveVisitStrategy = (visited) => visited.Values.Any(c => c > 1));
            VisitAll(start).Count().AsResult2();

            Report.End();
        }

        private static Dictionary<string, Cave> ConnectCaves(List<Path> foos, List<Cave> caves)
        {
            var lookup = caves.ToDictionary(k => k.Id, v => v);

            foreach (var cave in caves)
            {
                var neighbours1 = foos.Where(p => p.Start == cave.Id).Select(p => p.End);
                var neighbours2 = foos.Where(p => p.End == cave.Id).Select(p => p.Start);
                var neighbours = neighbours1
                        .Union(neighbours2)
                        .Distinct()
                        .Select(lookup.GetValueOrDefault)
                        .ToList();

                cave.Neighbours = neighbours;
            }

            return lookup;
        }

        private static IEnumerable<List<Cave>> VisitAll(Cave cave, Dictionary<Cave, int> visited = null)
        {
            if (visited == null)
                visited = new Dictionary<Cave, int>();

            if (cave.Id == "end") {
                yield return new List<Cave>(){cave};
                yield break;
            }
            if (!cave.CanVisit(visited)) {
                yield break;
            }
            if (!cave.IsBigCave) {
                var count = visited.TryGetDef(cave, 0) + 1;
                visited[cave] = count;
            }

            foreach (var neighbour in cave.Neighbours.Where(n => n.Id != "start"))
            {
                var dic = new Dictionary<Cave, int>(visited);
                var paths = VisitAll(neighbour, dic);
                foreach (var path in paths)
                {
                    path.Add(cave);
                    yield return path;
                }
            }
        }

        public static List<Path> LoadPaths(string inputTxt)
        {
            var paths = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^(.+)-(.+)$", m => new Path()
                {
                   Start = m.Groups[1].Value,
                   End = m.Groups[2].Value,
                }))
                .ToList();

            return paths;
        }
    }
}
