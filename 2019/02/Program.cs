using System;

using System.IO;
using System.Linq;

namespace day02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var fullString = File.ReadAllText("input.txt");
            var register = fullString.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(long.Parse).ToArray();

            var firstRegister = Calc(register, 12, 2);
            Console.WriteLine(firstRegister);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            Console.WriteLine("==== Part 2 ====");
            Console.WriteLine("Looking for noun & verb to produce 19690720");
            for (int noun = 0; noun < 100; noun++)
            {
                for (int verb = 0; verb < 100; verb++)
                {
                    var res = Calc(register, noun, verb);
                    if (res == 19690720) {
                        Console.WriteLine($"Noun: {noun}, Verb: {verb}");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return;
                    }
                }
            }
        }

        private static long Calc(long[] originalRegistry, int noun, int verb)
        {
            var register = (long[])originalRegistry.Clone();
            register[1] = noun;
            register[2] = verb;

            var pos = 0;
            var running = true;
            do
            {
                var opcode = register[pos];
                switch (opcode)
                {
                    case 1: //addition
                        var a = register[register[pos +1]];
                        var b = register[register[pos +2]];
                        var addRes = a + b;
                        register[register[pos + 3]] = addRes;
                        pos += 4;
                        break;
                    case 2: //multiplication
                        var x = register[register[pos +1]];
                        var y = register[register[pos +2]];
                        var multiRes = x * y;
                        register[register[pos + 3]] = multiRes;
                        pos += 4;
                        break;
                    case 99: //halt
                        running = false;
                        break;
                    default:
                        running = false;
                        Console.WriteLine($"ERROR: pos {pos} opcode {opcode}");
                        return 0;
                }
            } while (running);

            return register[0];
        }
    }
}
