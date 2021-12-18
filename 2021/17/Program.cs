using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo: IHasPosition<Point2>
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public Point2 Pos { get; set; }

    }

  [System.Serializable]
  public class MissedException : System.Exception
  {
      public Point2 Counter {get; set;}
        public int Moves { get; }

        public MissedException(Point2 counter, int moves) {
          this.Counter = counter;
            Moves = moves;
        }
  }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");


            var targets = NewMethod(foos).ToList();


            var start = new Point2(0, 0);
            "6,9".Debug("Sample Target velo for y45");


            var velo = new Point2(6,9);

            HashSet<Point2> targets1 = new HashSet<Point2>(targets.Select(f => f.Pos));
            //var tra = traject(start, velo, targets1).Peek("t");

            var attemtps = new HashSet<Point2>();
            var v = new Point2(0,200);
            var bestT = new Point2(-1, -1);
            while(true){
                
                try {
                    v.Debug("v-t");
                    var tra = traject(start, v, targets1).ToList();

                    if (v.Y > bestT.Y) {
                        bestT = new Point2(v.X, v.Y);
                        
                        v.X = 0;
                        break;
                    }
                }
                catch(MissedException m){
                    if(m.Counter != null){
                        v = v.Move(m.Counter.Debug("C"));
                        if(attemtps.Contains(v))
                            {
                                
                        v = v.Move(new Point2(0, -1));
                        attemtps.Clear();
                            }
                    } else {
                        v = v.Move(new Point2(0, -1));
                        attemtps.Clear();
                    }
                    attemtps.Add(v);
                }
            }

            var minY = targets1.Min(t => t.Y);
            v = new Point2(0,minY);
            var bestB = new Point2(-1, -1);
            attemtps.Clear();
            var fm= int.MinValue;
            while(true){
                
                try {
                    v.Debug("v-b");
                    var tra = traject(start, v, targets1, true).ToList();

                    //if (v.Y > bestB.Y) {
                        bestB = new Point2(v.X, v.Y);
                        
                        break;
                    //}
                }
                catch(MissedException m){
                    if(m.Counter != null){
                        v = v.Move(m.Counter.Debug("C"));
                        if(attemtps.Contains(v)){
                            
                        v = v.Move(new Point2(0, 1));
                        attemtps.Clear();
                        }
                    } else {
                        if(fm <= m.Moves){
                            fm.Debug("fm");
                            m.Moves.Debug("Moves");
                            fm = m.Moves;
                            v.X++;
                        } else {
                            v = v.Move(new Point2(0, 1));
                            attemtps.Clear();
                        }
                    }
                    attemtps.Add(v);
                }
            }


                
                    var bestTtra = traject(start, bestT, targets1);
                var field = new Field<Point2, Foo>(OutOfBoundsStrategy.CREATE_NEW);
                field.Add(new Foo(){Pos = new Point2(0,0), A= "S"});
                    field.Add(targets);
                    //field.Dic[target.Pos].A = "#";
                    field.Add(bestTtra.Select(t => new Foo(){ Pos = t, A="#"}));

                    field.ToConsole(f => f.A);
                    bestT.Debug("bestT");

                     var bestBtra = traject(start, bestB, targets1);
                var field2 = new Field<Point2, Foo>(OutOfBoundsStrategy.CREATE_NEW);
                field2.Add(new Foo(){Pos = new Point2(0,0), A= "S"});
                    field2.Add(targets);
                    //field.Dic[target.Pos].A = "#";
                    field2.Add(bestBtra.Select(t => new Foo(){ Pos = t, A="#"}));

                    field2.ToConsole(f => f.A);
                    bestB.Debug("bestB");


            var yy = Enumerable.Range(bestB.Y, bestT.Y-bestB.Y+1).SelectMany(y => findAll(y, targets1)).ToHashSet();
            
            targets.ForEach(f => yy.Add(f.Pos));

                    bestB.Debug("bestB");
                    bestT.Debug("bestT");
                    minY.Debug("minY");

                    
                    bestTtra.Max(t => t.Y).AsResult1();
                    yy.Count.AsResult2();

            Report.End();
        }

        private static List<Point2> findAll(int y, HashSet<Point2> targets1)
        {
            
            var v = new Point2(0, y);
            var vv = new List<Point2>();
            while(true){
                
                try {
                    v.Debug("v-f");
                    var tra = traject(new Point2(0,0), v, targets1, true).ToList();

                    vv.Add(new Point2(v.X, v.Y));
                    v.X ++;
                }
                catch(MissedException m){
                    if(m.Counter != null){
                        var nv = v.Move(m.Counter.Debug("CC"));
                        if(nv.X < v.X)
                            break;
                        v = nv;
                    } else {
                        //v.Debug("WTF");
                        
                            v.X ++;
                        break;
                    }
                }
            }
            return vv.Peek(y.ToString()).Distinct().ToList();
        }

        private static IEnumerable<Point2> traject(Point2 initPos, Point2 initVelo, HashSet<Point2> targets, bool bottom = false)
        {
            var minY = targets.Min(t => t.Y);
            var maxX = targets.Max(t => t.X);
            var minX = targets.Min(t => t.X);
            var newPos = initPos;
            var velo = new Point2(initVelo.X, initVelo.Y);
            var moves = 0;
            while(!targets.Contains(newPos)){
                var prev = newPos;
                newPos = newPos.Move(velo);
                moves++;
                yield return newPos;
                
                if(velo.X > 0)
                    velo.X--;
                if(velo.X < 0)
                    velo.X++;
                velo.Y--;


                    if(newPos.Y < minY && prev.X <= maxX)
                        throw new MissedException(new Point2(1,0), moves);
                    if(newPos.Y < minY && prev.X > maxX)
                        throw new MissedException(new Point2(-1,0), moves);
                    if(newPos.Y < minY)
                        throw new MissedException(null, moves);

                

            }
        }

        private static object calcVelo(Point2 start, Field<Point2, Foo> field, object step)
        {
          //  if(field.Dic.Contains())
          return null;
        }

        //The probe's x,y position starts at 0,0. Then, it will follow some trajectory by moving in steps. On each step, these changes occur in the following order:
//The probe's x position increases by its x velocity.
//The probe's y position increases by its y velocity.
//Due to drag, the probe's x velocity changes by 1 toward the value 0; that is, it decreases by 1 if it is greater than 0, increases by 1 if it is less than 0, or does not change if it is already 0.
//Due to gravity, the probe's y velocity decreases by 1.

        private static IEnumerable<Foo> NewMethod(Foo foos)
        {
            var xes = Enumerable.Range(foos.MinX, foos.MaxX - foos.MinX + 1).ToList();
            var ys = Enumerable.Range(foos.MinY, foos.MaxY - foos.MinY + 1).ToList();
            foreach (var x in xes)
            {
                foreach (var y in ys)
                {
                    
                    yield return new Foo(){Pos = new Point2(x,y), A= "T"};
                }
            }
        }

        public static Foo LoadFoos(string inputTxt)
        { //x=155..215, y=-132..-72
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Peek("Input")

             //.GroupByLineSeperator()
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
               .Select(s => s.ParseRegex(@"^target area: x=([-0-9]+)..([-0-9]+), y=([-0-9]+)..([-0-9]+)$", m => new Foo()
               {
                   MinX = int.Parse(m.Groups[1].Value),
                   MaxX = int.Parse(m.Groups[2].Value),
                   MinY = int.Parse(m.Groups[3].Value),
                   MaxY = int.Parse(m.Groups[4].Value),
               }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
             .ToList().First()
            ;

            return foos;
        }
    }
}
