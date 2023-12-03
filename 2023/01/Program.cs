using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");
            var rocks = new HashSet<Point2>();
            var sandSource = new Point2(500,0);

            foreach (var path in foos)
            {
                var waypoints = path.Splizz("->")
                    .Select(p => p.Splizz(",").Select(p => int.Parse(p)).ToList())
                    .Select(p => new Point2(p.First(), p.Last()))
                    .ToList();
                for (int i = 1; i < waypoints.Count; i++)
                {
                    var start = waypoints[i-1].Debug("start");
                    var end = waypoints[i].Debug("end");
                    if(start.X == end.X) 
                        Enumerable.Range(0, Math.Abs(start.Y-end.Y)+1)
                            .Select(y => new Point2(start.X, Math.Min(start.Y, end.Y) +y))
                            .ForEach(r => rocks.Add(r));
                    
                    if(start.Y == end.Y) 
                        Enumerable.Range(0,Math.Abs(start.X-end.X)+1)
                            .Select(x => new Point2(Math.Min(start.X, end.X)+x, end.Y))
                            .ForEach(r => rocks.Add(r));
                }
            }
            var abyss = rocks.Max(r => r.Y)+1;
            var sands = new HashSet<Point2>(rocks);
            sands.Count().Debug("INITS");
            var go = true;
            var a = 0;
            while(go){
                a++;
                var sand = sandSource;
                go = true;
                while(true) {
                    //sand.Debug("rel");
                    
                    //if(sand.Y > abyss+2) {go = false.Debug(abyss); break;}

                    if(!sands.Contains(sand.Down()) && !(sand.Down().Y > abyss))
                    {
                        sand = sand.Down();
                        continue;
                    }

                    var possis = new List<List<Point2>>(){
                        new List<Point2>(){sand.Left().Down() }, 
                        new List<Point2>(){sand.Right().Down() }
                    };
                    var free = possis.Where(path => path.All(p => !sands.Contains(p) && !(sand.Down().Y > abyss))).Select(path => path.First()).FirstOrDefault();
                    
                    //var next = possis.Debug().Where(p => !sands.Contains(p)).FirstOrDefault();
                    if(free == null){
                        //"bottom".Debug();
                        //Console.ReadKey();
                        //sands.ToConsole();
                        //go = false;
                        break;
                    }
                    
                    sand = free;
                    continue;
                }
                //go.Debug("go");
                //sand.Debug("sand");
                
                if(go)
                    if(sands.Add(sand.Debug("added")))
                        continue;
                    else break;
                
                //go.Debug("end loopw");
            }
            
            //sands.Except(rocks).ToConsole();

            sands.Except(rocks).Count().AsResult1();
            sands.Add(sandSource);
            //sands.Union(rocks).ToConsole(pr => pr == sandSource ? "+": rocks.Contains(pr) ? "#": "o");

            a.Debug("AAA");
            sands.Count().Debug("sand ocunt");
            rocks.ToCommaString().Debug("rocky rocks");

            Report.End();
        }

        public static List<string> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())


             //.GroupByLineSeperator()
             
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
             //      A = m.Groups[1].Value,
             //      B = m.Groups[2].Value,
             //  }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
             .ToList()
            ;

            return foos;
        }
    }
}
