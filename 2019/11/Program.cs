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

        static void Main(string[] args)
        {  //203
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var extraMemory = string.Join(",", (new string('0', 1000*10)).ToArray());
            register = Compile(File.ReadAllText("input.txt") +","+ extraMemory);
            
            var state = CalcOutput(new Point(0,0));
            int v = state.Panels.Values.Count(p => p.TimesPainted > 0);
            Console.WriteLine("panels painted: {0}", v);
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            

            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();

            var state2 = CalcOutput2(new Point(0,0));
            var rawpoints = state2.Panels
                .Where(kvp => kvp.Value.TimesPainted > 0 && kvp.Value.Color == "#")
                .Select(kvp => kvp.Key)
                .ToList();

            var minX = rawpoints.Min(p => p.X);
            var minY = rawpoints.Min(p => p.Y);

            Console.WriteLine($"Offest x {minX}, y {minY}");

            var canvas = rawpoints.
                Select(p => new Point(p.X - minX, p.Y -minY))
                .ToMultiDimArray(1);            

            canvas.ToConsole(x => x == 0 ? " ": "█");

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }
        
        private static State RunOpcodeProgram(State state)
        {
            var register = state.Register;
            var latestOut = 0L;
            string toPaint = null;
            string toTurn = null;
            do
            {
                var opcode = register[state.Pos].ToString().PadLeft(6, '0');
                var val = new ValueRetriever(state, opcode);
                //Console.WriteLine($"Current Pos: {state.Pos}");

                switch (opcode)
                {
                    case string o when o.EndsWith("01"): //addition
                        var a = val.Get(1);
                        var b = val.Get(2);
                        var addRes = a + b;
                        
                        //if (isDebug) Console.WriteLine($"Adding {a} + {b}: " + addRes);
                        val.Set(3, addRes);
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("02"): //multiplication
                        var x = val.Get(1);
                        var y = val.Get(2);
                        var multiRes = x * y;
                        //if (isDebug) Console.WriteLine($"Multi {x} + {y}: " + multiRes);
                        val.Set(3, multiRes);
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("03"): //input
                        Panel panelToCheck;
                        if(!state.Panels.TryGetValue(state.CurrentRobotPos, out panelToCheck)){
                            panelToCheck = new Panel();
                            state.Panels.Add(state.CurrentRobotPos, panelToCheck);
                        }
                        var input = panelToCheck.Color == "." ? 0 : 1;
                        
                        if (isDebug) Console.WriteLine("Input: " + input);
                        val.Set(1, input);
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        if (isDebug) Console.WriteLine("\r\nOut: " + output);
                        
                        if(toPaint == null) {
                            toPaint = output == 0 ? "." : "#";
                            Panel panelToPaint;
                            if(!state.Panels.TryGetValue(state.CurrentRobotPos, out panelToPaint)){
                                panelToPaint = new Panel();
                                state.Panels.Add(state.CurrentRobotPos, panelToPaint);
                            }
                            panelToPaint.Color = toPaint;
                            panelToPaint.TimesPainted++;
                        } else {
                            toPaint = null;
                            toTurn = output == 0 ? "left" : "right";
                            switch(state.Direction){
                                case "up":
                                    state.Direction = toTurn == "left" ? "left" : "right";
                                break;
                                
                                case "left":
                                    state.Direction = toTurn == "left" ? "down" : "up";
                                break;
                                
                                case "right":
                                    state.Direction = toTurn == "left" ? "up" : "down";
                                break;
                                
                                case "down":
                                    state.Direction = toTurn == "left" ? "right" : "left";
                                break;
                                default:
                                    throw new Exception("Facing unknown direction1!");
                            }

                            Func<Point,Point> step = null;
                            switch(state.Direction){
                                case "up":
                                    step = BuildStep('U');
                                break;
                                
                                case "left":
                                    step = BuildStep('L');
                                break;
                                
                                case "right":
                                    step = BuildStep('R');
                                break;
                                
                                case "down":
                                    step = BuildStep('D');
                                break;
                                default:
                                    throw new Exception("Facing unknown direction2!");
                            }
                            
                            state.CurrentRobotPos = step(state.CurrentRobotPos);
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
                        //if (isDebug) Console.WriteLine($"\r\n{oldRelativeBase} + {relativeBaseChange} = {state.RelativeBase}");
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("99"): //halt
                        if (isDebug) Console.WriteLine("\r\nHALT!");
                        //Console.WriteLine(string.Join(", ", state.Register));
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

        private static long[] Compile(string fullString)
        {
            return fullString
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Select(long.Parse).ToArray();
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
            state.Panels.Add(origin, new Panel());
            return RunOpcodeProgram(state);
        }    
        
        private static State CalcOutput2(Point origin)
        {
            var state = new State(){
                Name = "noop",
                Panels = new Dictionary<Point, Panel>(),
                Direction = "up",
                Register = (long[]) register.Clone(),
                Running = true,
                CurrentRobotPos = origin,
            };
            state.Panels.Add(origin, new Panel(){Color = "#"});
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
