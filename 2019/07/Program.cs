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
        private static bool isDebug = false; 

        static void Main(string[] args)
        { 
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            register = Compile(File.ReadAllText("input.txt"));

            var possiblePhases = new[] { 0L, 1L, 2L, 3L, 4L }.ToList();
            var permus = possiblePhases.GetPermutations(possiblePhases.Count);

            long max = permus.AsParallel()
                .Select(CalcOutput)
                .Max();

            Console.WriteLine("Largest thruster signal: {0}", max);

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();

            possiblePhases = new[] { 5L, 6L, 7L, 8L, 9L }.ToList();
            permus = possiblePhases.GetPermutations(possiblePhases.Count);

            long maxByLoop = permus.AsParallel()
                .Select(CalcLoopOutput)
                .Max();

            Console.WriteLine("Largest thruster signal with loop: {0}", maxByLoop);

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        
        private static long CalcLoopOutput(IEnumerable<long> phases, int arg2)
        {
            var ampl = "ABCDEF";
            var amps = phases
                .Select((phase, index) =>  new State(){ 
                    Name = ampl[index].ToString(),
                    Register = (long[])register.Clone(),
                    Phase = phase,
                    IsRunning = true
                }).ToArray();
            
            var activeAmplifier = 0;
            var signal = 0L;
            while (true)
            {
                var amplifier = amps[activeAmplifier];
                var newSignal = RunOpcodeProgram(amplifier, signal);
                if(amplifier.IsRunning) {
                    signal = newSignal;
                } else {
                    if (isDebug) Console.WriteLine("{0} stopped", amplifier.Name);
                    return signal;
                }

                activeAmplifier++;
                if(activeAmplifier >= amps.Length){
                    activeAmplifier = 0;
                }
            }
        }

        private static long RunOpcodeProgram(State state, long inputSignal)
        {
            var register = state.Register;
            var latestOut = 0L;
            do
            {
                var opcode = register[state.Pos].ToString().PadLeft(6, '0');
                var val = new ValueRetriever(register, opcode, state.Pos);

                switch (opcode)
                {
                    case string o when o.EndsWith("01"): //addition
                        var a = val.Get(1);
                        var b = val.Get(2);
                        var addRes = a + b;
                        register[register[state.Pos + 3]] = addRes;
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("02"): //multiplication
                        var x = val.Get(1);
                        var y = val.Get(2);
                        var multiRes = x * y;
                        register[register[state.Pos + 3]] = multiRes;
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("03"): //input
                        var input = state.SentPhase ? inputSignal : state.Phase;
                        state.SentPhase = true;
                        
                        if (isDebug) Console.WriteLine("Input: " + input);
                        register[register[state.Pos + 1]] = input;
                        state.Pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        if (isDebug) Console.WriteLine("\r\nOut: " + output);
                        latestOut = output;
                        state.Pos += 2;
                        return output;
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
                        register[register[state.Pos + 3]] =
                            val.Get(1) < val.Get(2)
                            ? 1
                            : 0;
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("08"): //equals
                        register[register[state.Pos + 3]] =
                            val.Get(1) == val.Get(2)
                            ? 1
                            : 0;
                        state.Pos += 4;
                        break;
                    case string o when o.EndsWith("99"): //halt
                        if (isDebug) Console.WriteLine("\r\nHALT!");
                        state.IsRunning = false;
                        break;
                    default:
                        state.IsRunning = false;
                        Console.WriteLine($"ERROR: pos {state.Pos} opcode {opcode}");
                        return -1;
                }
            } while (state.IsRunning);

            //unchanged input
            return inputSignal;
        }

        class State{
            public long[] Register {get; set;}
            public long Pos {get; set;}
            public bool IsRunning {get; set;}
            public long Phase {get; set;}
            public string Name { get; internal set; }
            public bool SentPhase { get; internal set; }
        }

        private static long[] Compile(string fullString)
        {
            return fullString
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Select(long.Parse).ToArray();
        }

        private static long CalcOutput(IEnumerable<long> phases, int arg2)
        {
            var ampl = "ABCDEF";

            return phases.Select((phase, index) => new State(){
                    Name = ampl[index].ToString(),
                    Phase = phase,
                    Register = (long[]) register.Clone(),
                    IsRunning = true
                })
                .Aggregate(0L, (acc, amp) => RunOpcodeProgram(amp, acc));
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

    internal class ValueRetriever
    {
        private readonly string accessModes;
        private readonly long[] _register;
        private readonly long _pos;

        public ValueRetriever(long[] register, string opcode, long pos)
        {
            accessModes = string.Join(string.Empty, opcode.Substring(0, opcode.Length -2).Reverse());
            _register = register;
            _pos = pos;
        }

        public long Get(int i)
        {
            var mode = GetMode(i);

            var val = _register[_pos + i];

            if (mode == "immidiate")
                return val;

            return _register[val];
        }

        private string GetMode(int i)
        {
            if (i >= accessModes.Length)
                return "position";
            var modeId = accessModes[i-1];
            return modeId == '0' ? "position" : "immidiate";
        }
    }
}
