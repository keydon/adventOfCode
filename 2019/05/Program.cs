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
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            Console.WriteLine("Provide Input of '1' when prompted!");

            var stopwatch = Stopwatch.StartNew();
            var fullString = File.ReadAllText("input.txt");
            var register = fullString
                .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(long.Parse).ToArray();


            RunOpcodeProgram(register);

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine("Provide Input of '5' when prompted!");

            
            RunOpcodeProgram(register);

            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }

        private static void RunOpcodeProgram(long[] originalRegister)
        {
            var register = (long[])originalRegister.Clone();
            var pos = 0L;
            var running = true;
            do
            {
                var opcode = register[pos].ToString().PadLeft(6, '0');
                var val = new ValueRetriever(register, opcode, pos);

                switch (opcode)
                {
                    case string o when o.EndsWith("01"): //addition
                        var a = val.Get(1);
                        var b = val.Get(2);
                        var addRes = a + b;
                        register[register[pos + 3]] = addRes;
                        pos += 4;
                        break;
                    case string o when o.EndsWith("02"): //multiplication
                        var x = val.Get(1);
                        var y = val.Get(2);
                        var multiRes = x * y;
                        register[register[pos + 3]] = multiRes;
                        pos += 4;
                        break;
                    case string o when o.EndsWith("03"): //input
                        Console.Write("Input: ");
                        var input = int.Parse(Console.ReadLine() ?? String.Empty);
                        register[register[pos + 1]] = input;
                        pos += 2;
                        break;
                    case string o when o.EndsWith("04"): //output
                        var output = val.Get(1);
                        Console.Write("\r\nOut: " + output);
                        pos += 2;
                        break;
                    case string o when o.EndsWith("05"): //jump if true
                        var jumpIfTrue = val.Get(1);
                        if (jumpIfTrue != 0)
                            pos = val.Get(2);
                        else
                            pos += 3;
                        break;
                    case string o when o.EndsWith("06"): //jump if false
                        var jumpIfFalse = val.Get(1);
                        if (jumpIfFalse == 0)
                            pos = val.Get(2);
                        else
                            pos += 3;
                        break;
                    case string o when o.EndsWith("07"): //less then
                        register[register[pos + 3]] =
                            val.Get(1) < val.Get(2)
                            ? 1
                            : 0;
                        pos += 4;
                        break;
                    case string o when o.EndsWith("08"): //equals
                        register[register[pos + 3]] =
                            val.Get(1) == val.Get(2)
                            ? 1
                            : 0;
                        pos += 4;
                        break;
                    case string o when o.EndsWith("99"): //halt
                        Console.WriteLine("\r\nHALT!");
                        running = false;
                        break;
                    default:
                        running = false;
                        Console.WriteLine($"ERROR: pos {pos} opcode {opcode}");
                        return;
                }
            } while (running);
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
