using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day11
{


    class Program
    {
        private static long[] register;
        private static bool isDebug = false;
        private static ConsoleKeyInfo key;
        private static string[,] res;
        private static bool isAutopilot = false;
        private static bool withVisuals = true;

        static void Main(string[] args)
        {

            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var extraMemory = string.Join(",", (new string('0', 1000 * 10)).ToArray());

            register = Compile(File.ReadAllText("input.txt") + "," + extraMemory);

            Console.WriteLine(
                "This solutions brings you in the game to manually discover the maze. \r\n"+
                "Press 'Y' to play. 'N' to skip to the result: ");
            var answer = Console.ReadKey().KeyChar.ToString().ToUpper();
            if (answer == "Y")
            {
                CalcOutput(new Point(0, 0));
            }
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");


            Console.WriteLine("==== Part 1.2 ====");
            stopwatch.Start();

            var superDictionary = new Dictionary<Point, Panel>();

            var panels = File.ReadAllLines("maze.txt")
                .SelectMany((row, y) => row.Select((character, x) => new Panel(superDictionary)
                {
                    Pos = new Point(x, y),
                    Color = character.ToString()
                })).ToList();

            foreach (var item in panels)
            {
                superDictionary.Add(item.Pos, item);
            }

            var start = panels.Single(p => p.Color == "D");
            var goal = panels.Single(p => p.Color == "O");
            var steps = new HashSet<Panel>() { start };

            var distance = Distance.Calculate(1, new Panel[] { start }, new List<Panel>() { goal }, steps);

            VisualizeSolutionPath(panels, start, goal, steps);

            stopwatch.Stop();
            Console.WriteLine("Steps : {0}", distance.GetDistance());
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();


            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();

            var fieldsWithoutOxygen = panels.Where(p => p.Color == ".").ToHashSet();
            fieldsWithoutOxygen.Add(start);
            var fieldWithOxygen = new HashSet<Panel>() { goal };

            var workingset = new HashSet<Panel>(fieldWithOxygen);
            var minutes = 0;
            while (fieldsWithoutOxygen.Any())
            {
                var newSpread = workingset
                    .SelectMany(p => p.GetNeighbours())
                    .Intersect(fieldsWithoutOxygen)
                    .ToHashSet();
                fieldsWithoutOxygen.RemoveWhere(p => newSpread.Contains(p));
                workingset = newSpread;
                minutes++;
            }

            stopwatch.Stop();
            Console.WriteLine("Minutes it took fully oxygenate: {0}", minutes);
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
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
                        long input = 0;
                        
                        if(withVisuals || !isAutopilot) 
                            DrawGame(state);
                        if (isAutopilot){
                            
                            switch (direction)
                            {
                                case "l": input = -1; break;
                                case "r": input = 1; break;
                                case " ": input = 0; break;
                            }
                            if (isDebug) Console.WriteLine("goin " + direction);
                        } else {
                            key = Console.ReadKey(true);
                            if(key.Key == ConsoleKey.UpArrow){
                                input = 1;
                                step = BuildStep('U');
                            } else if (key.Key == ConsoleKey.DownArrow)  
                            {
                                step = BuildStep('D');
                                input = 2;
                            }  else if (key.Key == ConsoleKey.LeftArrow)  
                            {
                                input = 3;
                                step = BuildStep('L');
                            }  else if (key.Key == ConsoleKey.RightArrow)  
                            {
                                input = 4;
                                step = BuildStep('R');
                            }
                         }
                        val.Set(1, input);
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);

                        if (isDebug) Console.WriteLine("\r\nOut: " + output);
                        switch (output)
                        {
                            case 0L:
                                var wall = step(state.CurrentRobotPos);
                                
                                if(!state.Panels.TryGetValue(wall, out var _)){
                                    state.Panels.Add(wall, new Panel(state.Panels){
                                        Color = "#",
                                        Pos = wall
                                    });
                                };
                                break;
                            case 1L:
                                var freeSpace = step(state.CurrentRobotPos);
                                if(!state.Panels.TryGetValue(freeSpace, out var _)){
                                    state.Panels.Add(freeSpace, new Panel(state.Panels){
                                    Color = ".",
                                    Pos = freeSpace
                                });
                                };
                                state.CurrentRobotPos = freeSpace;
                                break;
                            case 2L:
                                var oxy = step(state.CurrentRobotPos);
                                if(!state.Panels.TryGetValue(oxy, out var _)){
                                    state.Panels.Add(oxy, new Panel(state.Panels){
                                        Color = "O",
                                        Pos = oxy
                                    });
                                }
                                Console.WriteLine("Goal reached! {0}", oxy);
                                Console.ReadLine();
                                state.CurrentRobotPos = oxy;
                                break;
                            default:
                             throw new Exception("Unknown droid response: "+ output);
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
                        Console.WriteLine("\r\nGAME OVER!!!!!!!!!!!");
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

        private static State CalcOutput(Point origin)
        {
            var state = new State(){
                Name = "noop",
                Panels = new Dictionary<Point, Panel>(),
                Direction = "up",
                Register = (long[]) register.Clone(),
                Running = true,
                CurrentRobotPos = origin,
            };
            state.Panels.Add(origin, new Panel(state.Panels){
                Pos = origin,
                Color = "d"
            });
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
            yield return dic[new Point(Pos.X, Pos.Y - 1)];
            yield return dic[new Point(Pos.X - 1, Pos.Y)];
            yield return dic[new Point(Pos.X + 1, Pos.Y)];
            yield return dic[new Point(Pos.X, Pos.Y + 1)];
        }
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
