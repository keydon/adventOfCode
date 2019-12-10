using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day02
{
    class Program
    {
        private static long[] register;
        private static bool isDebug = true; 

        static void Main(string[] args)
        {  //203
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var extraMemory = string.Join(",", (new string('0', 1000*10)).ToArray());
            register = Compile(File.ReadAllText("input.txt") +","+ extraMemory);
            
            var boostKeyCode = CalcOutput(1L);
            Console.WriteLine("BOOST key code: {0}", boostKeyCode);
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();

            var coordinates = CalcOutput(2L);
            Console.WriteLine("Distress coordinates: {0}", coordinates);

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }
        
        private static long RunOpcodeProgram(State state, long inputSignal)
        {
            var register = state.Register;
            var latestOut = 0L;
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
                        var input = state.SentPhase ? inputSignal : state.Phase;
                        state.SentPhase = true;
                        
                        if (isDebug) Console.WriteLine("Input: " + input);
                        val.Set(1, input);
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        if (isDebug) Console.WriteLine("\r\nOut: " + output);
                        latestOut = output;
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
                        return -1;
                }
            } while (state.Running);

            return latestOut;
        }

        private static long[] Compile(string fullString)
        {
            return fullString
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Select(long.Parse).ToArray();
        }

        private static long CalcOutput(long initialInputSignal)
        {
            var state = new State(){
                Name = "noop",
                Phase = initialInputSignal,
                Register = (long[]) register.Clone(),
                Running = true
            };
            return RunOpcodeProgram(state, initialInputSignal);
        }        
    }

    public static class Extensions{
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }

    
     public   class State{
            public long[] Register {get; set;}
            public long Pos {get; set;}
            public bool Running {get; set;}
            public long Phase {get; set;}
            public string Name { get; internal set; }
            public bool SentPhase { get; internal set; }
            public long RelativeBase {get; set;}
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
