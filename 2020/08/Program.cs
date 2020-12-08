using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _08
{
    class ProgramCrashed : Exception { }

    record Instruction
    {
        public int Amount { get; set; }
        public string OpCode { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var instructions = LoadInstructions("input.txt");

            var result1 = ExecuteOpcodeProgram(instructions, -1, false);
            Console.WriteLine($"Part1-Result: {result1}");

            int result2 = int.MinValue;
            for (int i = 0; i < instructions.Count; i++)
            {
                try
                {
                    result2 = ExecuteOpcodeProgram(instructions, i, true);
                }
                catch (ProgramCrashed)
                {
                    // NOOP
                }
            }

            Console.WriteLine($"Part2-Result: {result2}");

        }

        private static int ExecuteOpcodeProgram(List<Instruction> originalInstructions, int i, bool swapMode)
        {
            List<Instruction> instructions;
            if (swapMode)
            {
                var instructionToTest = originalInstructions[i];
                if (instructionToTest.OpCode != "jmp" && instructionToTest.OpCode != "nop")
                {
                    throw new ProgramCrashed();
                }
                else
                {
                    instructions = new List<Instruction>(originalInstructions.Select(instr => instr with { }));
                    FlipInstruction(instructions[i]);
                }
            }
            else
            {
                instructions = originalInstructions;
            }

            var loopDetection = new HashSet<int>();

            var acc = 0;
            var pos = 0;
            while (true)
            {
                if (pos >= instructions.Count || pos < 0)
                {
                    return acc;
                }

                if (loopDetection.Contains(pos))
                {
                    if (swapMode)
                    {
                        throw new ProgramCrashed();
                    }
                    return acc;
                }
                loopDetection.Add(pos);

                var current = instructions[pos];

                if (current.OpCode == "nop")
                {
                    pos++;
                }
                else if (current.OpCode == "acc")
                {
                    acc += current.Amount;
                    pos++;
                }
                else if (current.OpCode == "jmp")
                {
                    pos += current.Amount;
                }
            }
        }

        private static void FlipInstruction(Instruction current)
        {
            if (current.OpCode == "jmp")
            {
                current.OpCode = "nop";
            }
            else
            {
                current.OpCode = "jmp";
            }
        }

        public static List<Instruction> LoadInstructions(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => ParseRegex(s));

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
        }

        private static Instruction ParseRegex(string line)
        {
            // 
            // acc +4
            // 
            Regex operationRegEx = new Regex(@"^(.+) ([\+\-0-9]+)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception($"No RegEx-Match for: '{line}'");

            return new Instruction()
            {
                Amount = int.Parse(match.Groups[2].Value),
                OpCode = match.Groups[1].Value,
            };
        }
    }
}
