using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Command
    {
        public int NumericValue { get; set; }

        public string CommandId { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var commands = LoadCommands("input.txt");
            var dist = 0;
            var depth = 0;
            foreach (var instr in commands)
            {
                switch(instr.CommandId){
                    case "up": depth -= instr.NumericValue;
                        break;
                    case "down": depth += instr.NumericValue;
                        break;
                    case "forward": dist += instr.NumericValue;
                        break;
                }
            }


            (dist * depth).AsResult1();
            Report.End();
        }

        public static List<Command> LoadCommands(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^(.+) (\d+)$", m => new Command(){
                   CommandId = m.Groups[1].Value,
                   NumericValue = int.Parse(m.Groups[2].Value),
                }))
                .ToList();

            return foos;
        }
    }
}
