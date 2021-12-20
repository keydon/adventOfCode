using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;


namespace aoc
{
    record Beacon : IHasPosition<Point3>
    {
        public Scanner Parent;

        public Point3 Pos { get; set; }

        public HashSet<Beacon> OverlapsWith { get; set;} = new HashSet<Beacon>();

        public IEnumerable<int> Fingerprint(){
            foreach (var b in Parent.Beacons)
            {
                if(b == this)
                    continue;
                var x = Math.Abs((Pos.X) - (b.Pos.X));
                var y = Math.Abs((Pos.Y) - (b.Pos.Y));
                var z = Math.Abs((Pos.Z) - (b.Pos.Z));
                var ps = (new [] {x,y,z}).OrderByDescending(a => a).ToList();
                
                yield return HashCode.Combine(ps[0], ps[1], ps[2]);
            }
        }

        public override int GetHashCode()
        {
            return OverlapsWith.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    class RelativeMatcher : IEqualityComparer<Beacon>
    {
        private int v;

        public RelativeMatcher(int v = 1)
        {
            this.v = v;
        }

        public bool Equals(Beacon a, Beacon b)
        {
            if(a.OverlapsWith == b.OverlapsWith){
                return true;
            }
            if(v == 2) 
                return false;
            var af = a.Fingerprint().ToList();
            var bf = b.Fingerprint().ToList();
            var overlapF = af.Intersect(bf).ToList();
            if (overlapF.Count >= 11){
                if(a.OverlapsWith == b.OverlapsWith)
                    return true;
                var backup = b.OverlapsWith.ToList();
                
                b.OverlapsWith = a.OverlapsWith;
                a.OverlapsWith.Add(a);
                a.OverlapsWith.Add(b);
                backup.ForEach(bak => a.OverlapsWith.Add(bak));
                backup.ForEach(bak => bak.OverlapsWith = a.OverlapsWith);

                return true;
            }
            
            return false;
        }

        public int GetHashCode([DisallowNull] Beacon beacon)
        {
            return 0;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            foos = LoadFoos("sample.txt");

            var scannerById = foos.ToDictionary(k => k.Id, v => v);

            var alreadyMerged = new HashSet<string>();
            Scanner merged = scannerById[0];
            foreach (var s in foos.Skip(1))
            {
                var overlap = merged.Beacons.Intersect(s.Beacons, new RelativeMatcher()).ToList();
                if(overlap.Count >= 12){                    
                    Console.WriteLine($"{merged.Id} overlapts with {s.Id} with {overlap.Count}");

                    var offset = calcOffset(overlap, s);
                }
            }
            //merged.Beacons.Count.AsResult1();
            foos.SelectMany(f => f.Beacons).Distinct(new RelativeMatcher(2)).Count().AsResult1();


            Report.End();
        }

        private static Point3 calcOffset(List<Beacon> overlap, Scanner s)
        {
            var rotations = rotate(s);
            var i = 0;
            foreach (var rotation in rotations)
            {
                i++;
                try{
                    var refOffset = overlap.Select(o => off(o)).Distinct().Single();
                    return refOffset.Debug("JACKPOT");
                }
                catch(InvalidOperationException e){
                    i.Debug("bad");
                }
            }
           
        }

        private static object rotate(Scanner s)
        {
            yield return s;
            
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        
                    }
                }
            }
        }

        private static Point3 off(Beacon refOffset)
        {
            var our = refOffset.OverlapsWith.Single(o => o.Parent.Id == refOffset.Parent.Id);
            var other = refOffset.OverlapsWith.Single(o => o.Parent.Id != refOffset.Parent.Id);
            var offX = our.Pos.X - other.Pos.X;
            var offY = our.Pos.Y - other.Pos.Y;
            var offZ = our.Pos.Z - other.Pos.Z;
            var offset = new Point3(offX, offY, offZ);
            return offset;
        }

        public static List<Scanner> LoadFoos(string inputTxt)
        {
            var scanners = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .ToList();

            return scanners.Select(scanner => {
                var id = scanner.First().ParseRegex(@"^--- scanner (\d+) ---$", m => new Scanner()
                {
                   Id = int.Parse(m.Groups[1].Value),
                }); 

                var beacons = scanner.Skip(1).Select(s => s.ParseRegex(@"^([-0-9]+),([-0-9]+),([-0-9]+)$", m => new Point3(
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(m.Groups[3].Value)
                )))
                .Select(p => new Beacon(){
                    Parent = id,
                    Pos = p
                })
                .ToList();

                id.Beacons = beacons;
                return id;
            })
            .ToList();
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
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

            //return foos;
        }
    }

    internal class Scanner
    {
        public Scanner()
        {
        }

        public int Id { get; set; }
        public List<Beacon> Beacons { get; internal set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
