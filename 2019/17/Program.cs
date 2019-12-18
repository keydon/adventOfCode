using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day17
{
    class Program
    {
        private static long[] register;
        private static bool isDebug = false;
        private static string[,] res;
        private static bool isAutopilot = false;

        static void Main(string[] args)
        {

            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var extraMemory = string.Join(",", (new string('0', 1000 * 10)).ToArray());

            register = Compile(File.ReadAllText("input.txt") + "," + extraMemory);

            CalcOutput(null);
            var dic = new Dictionary<Point,Panel>();

            List<Panel> panels = File.ReadAllLines("maze.txt")
            .SelectMany((row, y) => row.Select((c, x) => new Panel(dic){
                Color = c.ToString(),
                Pos = new Point(x,y)
            })).ToList();

            panels.ForEach(p => dic.Add(p.Pos, p));

            List<Panel> way = panels.Where(p => p.Color == "#").ToList();

            List<Panel> intersect = way.Where(intersection => intersection.GetNeighbours().All(p => way.Contains(p))).ToList();
            int sum = intersect.Select(i => i.Pos.X * i.Pos.Y).Sum();
            
            stopwatch.Stop();
            Console.WriteLine("Sum of alignment parameters : {0}", sum);
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");


            Console.WriteLine("==== Part 2.1 ====");
            stopwatch.Start();

            var movementInstructions = Walk(panels.Single(p => p.Color == "^")).ToList();
            Console.WriteLine("Full Movement : {0}", string.Join(", ", movementInstructions));

            // After visual examination of the full movement
            var a = "L,10,R,8,R,8";
            var b = "L,10,L,12,R,8,R,10";
            var c = "R,10,L,12,R,10";
            var movements = "A,A,B,C,B,C,B,C,C,A";
            var videofeed = false ? "y" : "n";

            
            Console.WriteLine("==== Part 2.2 ====");
            var inputs = new[] { movements, a, b, c, videofeed }; 
            var inputSequence = inputs
                .SelectMany(s => s.Select(c => (long)c).Append((long)'\n'));

            // Reset Movement Programming
            register[0] = 2;
            CalcOutput(inputSequence);

            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        
        enum FacingDirections { UP, LEFT, RIGHT, DOWN}

        private static IEnumerable<string> Walk(Panel panel)
        {
            var facing = FacingDirections.UP;
            Point relativeRight;
            Point relativeLeft;
            while(true) {
                switch (facing)
                {
                    case FacingDirections.UP:
                        relativeLeft = panel.Left;
                        relativeRight = panel.Right;
                        break;
                    case FacingDirections.DOWN:
                        relativeLeft = panel.Right;
                        relativeRight = panel.Left;
                        break;
                    case FacingDirections.LEFT:
                        relativeLeft = panel.Lower;
                        relativeRight = panel.Upper;
                        break;
                    case FacingDirections.RIGHT:
                        relativeLeft = panel.Upper;
                        relativeRight = panel.Lower;
                        break;
                    default:
                        throw new Exception("fuck me");
                }

                var right = panel.GetNeighour(relativeRight) ?? new Panel(null){Color = "X"};
                var left = panel.GetNeighour(relativeLeft) ?? new Panel(null){Color = "X"};

                if(right.Color != "#" && left.Color != "#")
                    yield break;

                var turn = right.Color == "#" ? "R" : "L";
                switch (facing)
                {
                    case FacingDirections.UP:
                        facing = turn == "R"
                            ? FacingDirections.RIGHT
                            : FacingDirections.LEFT;
                        break;
                    case FacingDirections.DOWN:
                        facing = turn == "R"
                            ? FacingDirections.LEFT
                            : FacingDirections.RIGHT;
                        break;
                    case FacingDirections.LEFT:
                        facing = turn == "R"
                            ? FacingDirections.UP
                            : FacingDirections.DOWN;
                        break;
                    case FacingDirections.RIGHT:
                        facing = turn == "R"
                            ? FacingDirections.DOWN
                            : FacingDirections.UP;
                        break;
                    default:
                        throw new Exception("fuck me twice");
                }

                yield return turn;


                var steps = 0;
                var lastValidPanel = panel;
                while(true){
                    Point forward = GetPointInfrontOfRobot(lastValidPanel, facing);
                    var testPanel = lastValidPanel.GetNeighour(forward) ?? new Panel(null){Color = "X"};;
                    if(testPanel.Color != "#")
                        break;
                    steps++;
                    lastValidPanel = testPanel;
                }
                panel = lastValidPanel;
                yield return steps.ToString();
            }
        }

        private static Point GetPointInfrontOfRobot(Panel panel, FacingDirections facing)
        {
            Point forward;
            switch (facing)
            {
                case FacingDirections.UP:
                    forward = panel.Upper;
                    break;
                case FacingDirections.DOWN:
                    forward = panel.Lower;
                    break;
                case FacingDirections.LEFT:
                    forward = panel.Left;
                    break;
                case FacingDirections.RIGHT:
                    forward = panel.Right;
                    break;
                default:
                    throw new Exception("fuck me");
            }

            return forward;
        }

        private static void VisualizeSolutionPath(List<Panel> panels, Panel start, Panel goal, HashSet<Panel> steps)
        {
            var walls = panels
                            .Where(p => p.Color == "#")
                            .Select(p => p.Pos)
                            .ToMultiDimArray("█");
            var space = panels
                .Where(p => p.Color == ".")
                .Select(p => p.Pos)
                .ToMultiDimArray(" ", walls);

            var path = steps
                .Select(p => p.Pos)
                .ToMultiDimArray("*", space);
            (new List<Point>() { start.Pos }).ToMultiDimArray("D", space);
            (new List<Point>() { goal.Pos }).ToMultiDimArray("O", space);

            path.ToConsole();
        }

        private static State RunOpcodeProgram(State state)
        {
            var register = state.Register;
            Func<Point, Point> step = null;
            string direction = " ";
            do
            {
                var opcode = register[state.Pos].ToString().PadLeft(6, '0');
                var val = new ValueRetriever(state, opcode);

                switch (opcode)
                {
                    case string o when o.EndsWith("01"): //addition
                        var a = val.Get(1);
                        var b = val.Get(2);
                        var addRes = a + b;
                        
                        if (isDebug) Console.WriteLine($"Adding {a} + {b}: " + addRes);
                        val.Set(3, addRes);
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("02"): //multiplication
                        var x = val.Get(1);
                        var y = val.Get(2);
                        var multiRes = x * y;
                        if (isDebug) Console.WriteLine($"Multi {x} + {y}: " + multiRes);
                        val.Set(3, multiRes);
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("03"): //input
                        state.InputSequence.MoveNext();
                        Console.Write((char)state.InputSequence.Current);
                        val.Set(1, state.InputSequence.Current);
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        if (output <= char.MaxValue) {
                            Console.Write((char)output);
                        } else {
                            Console.Write(output);
                        }
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("05"): //jump if true
                        var jumpIfTrue = val.Get(1);
                        if (jumpIfTrue != 0)
                            state.Pos = val.Get(2);
                        else
                            state.Pos += 3;
                        break;
                    case string o when o.EndsWith("06"): //jump if false
                        var jumpIfFalse = val.Get(1);
                        if (jumpIfFalse == 0)
                            state.Pos = val.Get(2);
                        else
                            state.Pos += 3;
                        break;
                    case string o when o.EndsWith("07"): //less then
                        val.Set(3,
                            val.Get(1) < val.Get(2)
                            ? 1
                            : 0
                        );
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("08"): //equals
                        val.Set(3,
                            val.Get(1) == val.Get(2)
                            ? 1
                            : 0
                        );
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("09"): //setting rel base
                        var oldRelativeBase = state.RelativeBase;
                        var relativeBaseChange = val.Get(1);
                        state.RelativeBase = oldRelativeBase + relativeBaseChange;
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("99"): //halt
                        Console.WriteLine("\r\nHALT!");
                        state.Running = false;
                        break;
                    default:
                        state.Running = false;
                        Console.WriteLine($"ERROR: pos {state.Pos} opcode {opcode}");
                        return state;
                }
            } while (state.Running);

            return state;
        }

        private static void DrawGame(State state)
        {
            Console.Clear();
                var points = state.Panels.Values.Select(vals => vals.Pos);
                
            var maxY = points.Max(p => p.Y);
            var maxX = points.Max(p => p.X);
            var minX = points.Min(p => p.X);
            var minY = points.Min(p => p.Y);
            Console.WriteLine($"dims [{maxX},{maxY}]");
            if(minX < 0){
                maxX += Math.Abs(minX);
            }
            if(minY < 0){
                maxY += Math.Abs(minY);
            }

            var res = new string[maxX+1,maxY+1];

            

            var v = state.Panels.Values
                .GroupBy(p => p.Color, (k,p) => new{ k, P=p.Select(pr => pr.Pos).ToList()})
                .ToList();

            foreach (var item in v)
            {
                item.P
                    .Select(p => new Point(p.X -minX, p.Y -minY))
                    .ToMultiDimArray(item.k, res);
            }
            (new List<Point>{state.CurrentRobotPos})
                .Select(p => new Point(p.X -minX, p.Y -minY))
                .ToMultiDimArray("D", res);
             
            //Console.Clear();
            Console.SetCursorPosition(0,0);
            res.ToConsole();
        }

        private static long[] Compile(string fullString)
        {
            var reg = fullString
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(long.Parse).ToArray();
            return reg;
        }

        private static State CalcOutput(IEnumerable<long> inputSequence)
        {
            var state = new State(){
                Name = "noop",
                Panels = new Dictionary<Point, Panel>(),
                Direction = "up",
                Register = (long[]) register.Clone(),
                Running = true,
                InputSequence = inputSequence?.GetEnumerator()
            };
            return RunOpcodeProgram(state);
        }

        private static Func<Point,Point> BuildStep(char direction)
        {
            switch (direction)
            {
                case 'L':
                    return (p) => new Point(p.X - 1, p.Y);
                case 'R':
                    return (p) => new Point(p.X + 1, p.Y);
                case 'D':
                    return (p) => new Point(p.X, p.Y - 1);
                case 'U':
                    return (p) => new Point(p.X, p.Y + 1);
            }
            throw new Exception("unknown direction " + direction);
        }
    }

    public static class Extensions{
        
    public static T[,] ToMultiDimArray<T>(this IEnumerable<Point> points, T symbol, T[,] arr = null)
    {
        if (arr == null)
        {
            var maxY = points.Select(p => p.Y).Max();
            var maxX = points.Select(p => p.X).Max();
            Console.WriteLine($"dims [{maxX},{maxY}]");
            arr = new T[maxX +1,maxY +1];
        }
        points.ToList().ForEach(p => arr[p.X, p.Y] = symbol);
        return arr;
    }

    public static void ToConsole<T>(this T[,] canvas, Func<T, string> tranform = null){
        var innerTransform = tranform ?? new Func<T, string>( (x) => x?.ToString() ?? "?");

        Console.WriteLine($"\r\n[{canvas.GetUpperBound(0)+1},{canvas.GetUpperBound(1)+1}]");
        for (int y = canvas.GetUpperBound(1); y >= 0; y--)
        {
            for (int x = 0; x <= canvas.GetUpperBound(0); x++)
            {
                Console.Write(innerTransform(canvas[x,y]));
            }
            Console.Write("\r\n");
        }
        Console.Write("\r\n");
    }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }

   public class Panel{
        private readonly Dictionary<Point, Panel> allPanels;

        public Panel(Dictionary<Point, Panel> allPanels){
            TimesPainted = 0;
            Color = ".";
            this.allPanels = allPanels;
        }
       public int TimesPainted {get; set;}
       public string Color {get; set;}
        public Point Pos { get; internal set; }

        public IEnumerable<Panel> GetNeighbours()
        {
            var dic = allPanels;
            if (dic.TryGetValue(Upper, out var upper))
                yield return upper;
            if (dic.TryGetValue(Left, out var left))
                yield return left;
            if (dic.TryGetValue(Right, out var right))
                yield return right;
            if (dic.TryGetValue(Lower, out var lower))
                yield return lower;
        }

        public Panel GetNeighour(Point coordinates){
            if (allPanels.TryGetValue(coordinates, out var target))
                return target; 
            return null;
        }

        public Point Left => new Point(Pos.X - 1, Pos.Y);
        public Point Upper => new Point(Pos.X, Pos.Y - 1);
        public Point Right => new Point(Pos.X + 1, Pos.Y);
        public Point Lower => new Point(Pos.X, Pos.Y + 1);
    }

    
     public   class State{
            public long[] Register {get; set;}
            public long Pos {get; set;}
            public bool Running {get; set;}
            public string Name { get; internal set; }
            public long RelativeBase {get; set;}
        public string Direction { get; internal set; }
        public Dictionary<Point, Panel> Panels { get; internal set; }
        public Point CurrentRobotPos { get; internal set; }
        public long Score { get; internal set; }
        public Panel Paddle { get; internal set; }
        public IEnumerator<long> InputSequence { get; internal set; }
    }

    internal class ValueRetriever
    {
        private readonly string accessModes;
        private readonly long[] _register;
        private readonly long _pos;
        private readonly State state;

        public ValueRetriever(State state, string opcode)
        {
            accessModes = string.Join(string.Empty, opcode.Substring(0, opcode.Length -2).Reverse());
            _register = state.Register;
            _pos = state.Pos;
            this.state = state;
        }

        public long Get(int i)
        {
            var mode = GetMode(i);

            if (mode == "immidiate")
                return _register[_pos + i];
            
            var val = mode == "position"
                ? _register[_pos + i]
                : _register[_pos + i]+ state.RelativeBase;
            try {
                //Console.WriteLine($"Getting {_register[val]} at {val}");
                return _register[val];
            } catch(Exception e){
                Console.WriteLine($"mode: {mode} {i}, {val}, rel:{state.RelativeBase}, bound:{_register.Length}: {e.Message}");
                throw;
            };
        }

        public void Set(int posi, long value){
            var mode = GetMode(posi);

            if (mode == "immidiate")
                throw new Exception("Writing in immidiate mode");
            
            var val = mode == "position"
                ? _pos + posi
                : _pos + posi ;
            var posref = _register[val];
            //Console.WriteLine($"writing {value} at {posref} (pos: {_pos}, rel: { state.RelativeBase}, mode: {mode})");
                _register[mode == "position" ? posref :posref+ + state.RelativeBase ] = value;
        }

        private string GetMode(int i)
        {
            if (i >= accessModes.Length)
                return "position";
            var modeId = accessModes[i-1].ToString();

            if(modeId == "0")
                return "position";
            if(modeId == "1")
                return "immidiate";
            if(modeId == "2")
                return "relative";
            throw new Exception($"UNKNOWN position mode '{modeId}'" );
        }
    }

     internal class Distance
    {
        private readonly int _wave;

        public Distance(int wave, HashSet<Panel> steps, IEnumerable<Panel> oneStepFurther)
        {
            _wave = wave;
            Steps = steps;
            OneStepFurther = oneStepFurther;
        }

        public IEnumerable<Panel> OneStepFurther { get; set; }

        public HashSet<Panel> Steps { get; set; }

        public int GetDistance()
        {
            return _wave;
        }

        public Panel GetFirstStep(IEnumerable<Panel> getNeighbours)
        {
            return OneStepFurther.Intersect(getNeighbours)
                .OrderBy(x => x.Pos.Y)
                .ThenBy(x => x.Pos.X)
                .FirstOrDefault();
        }


        public static Distance Calculate(int i, IEnumerable<Panel> units, List<Panel> targetPanels, HashSet<Panel> steps)
        {
            var newSteps = units.SelectMany(u => u.GetNeighbours())
                .Where(p => p.Color != "#")
                .Except(steps)
                .ToHashSet();

            if (newSteps.Count() == 0) return null;


            steps.UnionWith(newSteps);
            if (newSteps.Overlaps(targetPanels))
            {
                var closerUnits = targetPanels
                    .Intersect(newSteps)
                    .OrderBy(y => y.Pos.Y)
                    .ThenBy(x => x.Pos.X)
                    .Take(1);
                return new Distance(i, steps, closerUnits);
            }
            var dis = Calculate(i + 1, newSteps, targetPanels, steps);
            if (dis == null) return null;
            var anotherStepFurther = dis.OneStepFurther.SelectMany(s => s.GetNeighbours())
                .Intersect(newSteps);
            return new Distance(dis.GetDistance(), steps, anotherStepFurther);
        }
    }

}
