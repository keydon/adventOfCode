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
        private static char[,] res;
        private static bool isAutopilot = true;
        private static bool withVisuals = true;

        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var extraMemory = string.Join(",", (new string('0', 1000*10)).ToArray());
      
            register = Compile(File.ReadAllText("input.txt") +","+ extraMemory);
            
            var statePartOne = CalcOutput(new Point(0,0));
            var v = statePartOne.Panels.Values
                .GroupBy(p => p.Color, (tile,p) => new{ Tile=tile, Occurrences = p.Count() })
                .OrderBy(g => g.Tile)
                .ToList();

            foreach (var item in v)
            {
                Console.WriteLine("Tile {0} exists {1} times", item.Tile, item.Occurrences);
            }
            
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            
            
            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            register[0] = 2; // Patch
            var statePartTwo = CalcOutput(new Point(0,0));

            stopwatch.Stop();
            Console.WriteLine("Highscore : {0}", statePartTwo.Score);
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        
        private static State RunOpcodeProgram(State state)
        {
            var inputP = new Point(-1,0);
            var register = state.Register;
            int? toX = null;
            int? toY = null;
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
                            if(key.Key == ConsoleKey.LeftArrow){
                            input = -1;
                            } else if (key.Key == ConsoleKey.RightArrow)  
                            {
                                input = 1;
                            }
                         }
                        val.Set(1, input);
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        if (isDebug) Console.WriteLine("\r\nOut: " + output);
                        
                        if(toX == null) {
                            toX = (int)output;
                        } else if (toY == null) {
                            toY = (int)output;
                        } else {

                           var p = new Point(toX.Value, toY.Value);
                            toX = null;
                            toY = null;

                            if(inputP == p){
                                state.Score = output;
                            } else {

                                Panel panelToPaint;
                                if(!state.Panels.TryGetValue(p, out panelToPaint)){
                                    panelToPaint = new Panel();
                                    state.Panels.Add(p, panelToPaint);
                                }
                                panelToPaint.Color = output.ToString();
                                panelToPaint.TimesPainted++;
                                var pp = panelToPaint.Pos;
                                panelToPaint.Pos = p;

                                if(output == 3L){
                                    state.Paddle = panelToPaint;
                                } else if(output == 4L){
                                    direction = p.X > state.Paddle?.Pos.X
                                        ? "r"
                                        : p.X < state.Paddle?.Pos.X
                                        ? "l"
                                        : pp.X < p.X
                                        ? "r"
                                        : pp.X > p.X
                                        ? "l" : " ";
                                }
                            }
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
            if(res==null){
                var points = state.Panels.Keys;
                var maxX = points.Max(p => p.X);
                var maxY = points.Max(p => p.Y);
                res = new char[maxX+1,maxY+1];
            }

            var v = state.Panels.Values
                .GroupBy(p => p.Color, (k,p) => new{ k, P=p.Select(pr => pr.Pos).ToList()})
                .ToList();

            foreach (var item in v)
            {
                char tile = ' ';
                switch (item.k)
                {
                    case "0":
                        tile = ' ';
                        break;
                    case "1":
                        tile = '█';
                        break;
                    case "2":
                        tile = 'X';
                        break;
                    case "3":
                        tile = '^';
                        break;
                    case "4":
                        tile = 'O';
                        break;     
                }

                item.P.ToMultiDimArray(tile, res);
            }
             
            //Console.Clear();
            Console.SetCursorPosition(0,0);
            Console.WriteLine("Score: {0}", state.Score);
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
            //state.Panels.Add(origin, new Panel());
            return RunOpcodeProgram(state);
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
        var innerTransform = tranform ?? new Func<T, string>( (x) => x.ToString());

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
        public Panel(){
            TimesPainted = 0;
            Color = ".";
        }
       public int TimesPainted {get; set;}
       public string Color {get; set;}
        public Point Pos { get; internal set; }
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
}
