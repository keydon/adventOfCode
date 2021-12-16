using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Paths
    {
        public Paths(Foo<Point2> start){
            var init = new Path();
            init.Add(start);
            Active.Add(init);
        }

        public void AddCompleted(Path path){
            AddActive(path);
            Active.Remove(path);
            Completed.Add(path);
            var min = Completed.Min(p => p.TotalCost);
            Best = Completed.First(p => p.TotalCost == min);
            Best.Debug("New Best!");
            Active.RemoveWhere(p => p.TotalCost >= Best.TotalCost);
        }

        public HashSet<Path> Active {get; set;} = new HashSet<Path>();
        public HashSet<Path> Completed {get; set;} = new HashSet<Path>();

        public Path Best {get; set; } 

        public Dictionary<Foo<Point2>, int> HighScores {get; set;} = new Dictionary<Foo<Point2>, int>();

        internal bool AddActive(Path path)
        {
            if( Best != null && path.TotalCost > Best.TotalCost) {
                Active.Remove(path);
                //path.Debug("drop");
                return false;
            }
            if (HighScores.TryGetValue(path.Head, out var best)){
                if (best > path.TotalCost){
                    HighScores[path.Head] = path.TotalCost;
                    Active.Add(path);
                    return true;
                } else {
                    Active.Remove(path);
                    return false;
                }
            }

            HighScores[path.Head] = path.TotalCost;
            Active.Add(path);
            return true;
        }
    }
    record Path{
        public Path(){
            Foos = new HashSet<Foo<Point2>>();
            TotalCost = 0;
        }
        public Path(Path path){
            Foos = new HashSet<Foo<Point2>>(path.Foos);
            TotalCost = path.TotalCost;
            Head = path.Head;
        }
        public HashSet<Foo<Point2>> Foos {get; set;}
        public int TotalCost {get; set;}
        public Foo<Point2> Head { get; internal set; }

        public bool Contains(Foo<Point2> foo){
            return Foos.Contains(foo);
        }
        public void Add(Foo<Point2> foo){
            Foos.Add(foo);
            TotalCost += foo.RiskLevel;
            Head = foo;
        }

        public override string ToString()
        {
            return $"Distance:{Foos.Count}, Head:{Head}, Total: {TotalCost}";
        }
    }
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int RiskLevel { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

        public override int GetHashCode()
        {
            return Pos.GetHashCode();
        }

        public override string ToString()
        {
            return Pos.ToString() + ":" + RiskLevel;
        }
    }

    enum Typi {
        SUM = 0,
        PRODUCT = 1,
        MIN = 2,
        MAX = 3,
        LITERAL = 4,
        GREATETHAN = 5,
        LESSTHEN = 6,
        EQUALTO = 7
    }

    record Packet
    {
        public long Version { get; set; }
        public Typi Typi { get; set; }

        public long Number {get; set; }
        public int Lenght { get; set; }

        public List<Packet> SubPackets {get; set;}
        public string RawPayload {get; set; }
        public int B { get; set; }
        public int C { get; set; }


    }
    class Program
    {
        static private int bestRoute = int.MaxValue;
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos2("input.txt");
            //foos = LoadFoos2("sample5.txt");
            //foos = "04005AC33890".Select(c => c).ToList();
            foos.Peek().ToList();
var map = new Dictionary<char, string>()
{
    {'0', "0000"},
{'1', "0001"},
{'2', "0010"},
{'3', "0011"},
{'4', "0100"},
{'5', "0101"},
{'6', "0110"},
{'7', "0111"},
{'8', "1000"},
{'9', "1001"},
{'A', "1010"},
{'B', "1011"},
{'C', "1100"},
{'D', "1101"},
{'E', "1110"},
{'F', "1111"},
};
        var inp = foos.Select(c => map[c]).ToCommaString("");
        inp.Debug("inp");
        var inpr= new Stack<char>(inp.Reverse().ToList());

        var pakets = ParsePacket(inpr);
        var sum = sumVersion(pakets);
        sum.AsResult1();

        var inpr2= new Stack<char>(inp.Reverse().ToList());

        var pakets2 = ParsePacket(inpr2);
        sumValue(pakets2).AsResult2();


            Report.End();
        }

        private static long sumValue(Packet p)
        {
            switch(p.Typi){
                case Typi.LITERAL: return p.Number;
                case Typi.SUM: return p.SubPackets.Select(s => sumValue(s)).Sum();
                case Typi.MIN: return p.SubPackets.Select(s => sumValue(s)).Min();
                case Typi.MAX: return p.SubPackets.Select(s => sumValue(s)).Max();
                case Typi.PRODUCT: return p.SubPackets.Select(s => sumValue(s)).MultiplyAll();
                case Typi.GREATETHAN: return sumValue(p.SubPackets[0]) > sumValue(p.SubPackets[1]) ? 1L : 0;
                case Typi.LESSTHEN: return sumValue(p.SubPackets[0]) < sumValue(p.SubPackets[1]) ? 1L : 0;  
                case Typi.EQUALTO: return sumValue(p.SubPackets[0]) == sumValue(p.SubPackets[1]) ? 1L : 0;  
                default: throw new Exception("Cannot sum" + p.Typi);
            }
        }

          private static long sumVersion(Packet pakets)
        {
            if(pakets.SubPackets != null && pakets.SubPackets.Count >0){
                return pakets.Version + pakets.SubPackets.Select(p => sumVersion(p)).Sum();
            }
            return pakets.Version;
        }

        private static Packet ParsePacket(Stack<char> inpr)
        
        {
                        
            var version = Enumerable.Range(0, 3).Select(x => inpr.Pop()).ToCommaString("").FromBinaryToLong().Debug("version");
            var t = Enumerable.Range(0, 3).Select(x => inpr.Pop()).ToCommaString("").FromBinaryToLong().Debug("t");
            var typi =  (Typi)t;
            switch (typi) { 
                case Typi.LITERAL: return ParseLiteral(version, inpr);
                default: return ParseOperator(version, inpr, typi);
            }
            typi.Debug("typi");
            
        }

        private static Packet ParseOperator(long version, Stack<char> inpr, Typi typi)
        {
            var indi = inpr.Pop();
            var lengthlength = indi == '0' ? 15 : 11;

            var length = Enumerable.Range(0, lengthlength).Select(x => inpr.Pop()).ToCommaString("").FromBinaryToLong().Debug("length");
            
            var subs = new List<Packet>();
            if(indi == '0'){
                var payload = Enumerable.Range(0, (int)length).Select(x => inpr.Pop()).ToCommaString("").Debug("payload");
                var subpayload = new Stack<char>(payload.Reverse());
                
                while(subpayload.TryPeek(out char res)){
                    subs.Add(ParsePacket(subpayload));
                }
            } 
            else {
                subs = Enumerable.Range(0, (int)length).Select(x => ParsePacket(inpr)).ToList();
            }

            return new Packet(){
                Version = version,
                Typi = typi,
                SubPackets = subs
            };
        }

        private static Packet ParseLiteral(long version, Stack<char> inpr)
        {
            var labels = new List<string>();
            var i = -1;
            while(true){
                var indi = inpr.Pop();
                i++;
                var lab = Enumerable.Range(0, 4).Select(x => inpr.Pop()).ToCommaString("").Debug("1st").Debug("l"+i);
                labels.Add(lab.ToString());

                if(indi == '0') break;
            }
            var num = labels.ToCommaString("").FromBinaryToLong().Debug("num");

            return new Packet(){
                Version = version,
                Typi = Typi.LITERAL,
                Number = num
            };
        }

        static void Main2(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            var field = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(foos);

            var start = field.Dic[new Point2(field.MinX,field.MinY)];
            var end = field.Dic[new Point2(field.MaxX,field.MaxY)];
            
            var pathz = new Paths(start);
            goAllWaysInWaves(field, pathz, end);
            (pathz.Best.TotalCost- start.RiskLevel).AsResult1();

            field.ToConsole(f => pathz.Best.Contains(f) ? "X": " ");
    
            var bigfield = new Field<Point2, Foo<Point2>>(OutOfBoundsStrategy.RETURN_NULL);
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    var incr = y+x;
                    bigfield.Add(
                        foos.Select(f => new Foo<Point2>(){
                            Pos = new Point2(field.MaxX * x +x + f.Pos.X, field.MaxY * y +y+ f.Pos.Y),
                            RiskLevel = (f.RiskLevel + incr) % 10 + (f.RiskLevel + incr) / 10
                        })
                    );
                }
            }

            //bigfield.ToConsole(x => x.RiskLevel.ToString());
            
            var start2 = bigfield.Dic[new Point2(bigfield.MinX,bigfield.MinY)];
            var end2 = bigfield.Dic[new Point2(bigfield.MaxX,bigfield.MaxY)];
                        
            var pathz2 = new Paths(start2);
            goAllWaysInWaves(bigfield, pathz2, end2);
            (pathz2.Best.TotalCost - start2.RiskLevel).AsResult2();

            Report.End();
        }


        private static void goAllWaysInWaves(Field<Point2, Foo<Point2>> field, Paths paths, Foo<Point2> end)
        {
            var i = 0;
            while(true){
                i++;
                var closest = paths.Active
                    .OrderByDescending(p => p.Head.Pos.ManhattenDistance(end.Pos))
                    .FirstOrDefault();
                if (closest == null){
                    i.Debug("tests");
                    return;
                }
            
                var neighbours = field.GetSimpleNeighbours(closest.Head).ToList();
                paths.Active.Remove(closest);
                foreach (var neighbour in neighbours)
                {
                    var newPath = new Path(closest);
                    newPath.Add(neighbour);
                    if (neighbour == end){
                        paths.AddCompleted(newPath);
                    } else {
                        paths.AddActive(newPath);
                    }
                }
            }
        }

        private static IEnumerable<HashSet<Foo<Point2>>> goAllWaysRecur(Field<Point2, Foo<Point2>> field, Dictionary<Point2, int> dic, int count, HashSet<Foo<Point2>> path, Foo<Point2> start, Foo<Point2> end)
        {
            if (path.Contains(start)){
                yield break;
            }

            var newCount = count + start.RiskLevel;
            if(newCount > bestRoute){
                yield break;
            }
            var shortestKnownCount = dic.TryGetDef(start.Pos, int.MaxValue);
            if(newCount > shortestKnownCount){
                yield break;
            }

            dic[start.Pos] = newCount;
            path.Add(start);

            if (start == end){
                bestRoute = newCount;
                yield return path;
                yield break;
            }

            foreach (var n in field.GetSimpleNeighbours(start).OrderBy(n => n.Pos.ManhattenDistance(end.Pos)).AsParallel())
            {
                var subpaths = goAllWaysRecur(field, dic, newCount, new HashSet<Foo<Point2>>(path), n, end);
                foreach (var subpath in subpaths)
                {
                    yield return subpath;
                }
            }

        }

        public static List<Foo<Point2>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             //.GroupByLineSeperator()
             .Parse2DMap((p, t) => new Foo<Point2> { Pos = p, RiskLevel = int.Parse(t) })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
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

         public static List<char> LoadFoos2(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .First().Select(c =>c)

             //.GroupByLineSeperator()
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
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
