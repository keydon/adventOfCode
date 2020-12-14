using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextCopy;


namespace aoc
{
    record OperationGroup
    {
        public string Mask;
        public List<Operation> Operations;
    }
    record Operation
    {
        public int Target { get; set; }
        public int Source { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample2.txt");

            foos.Aggregate(
                new Dictionary<long, long>(),
                (memory, group) =>
                {
                    group.Operations.ForEach(op => CalcPart1(memory, group.Mask, op));
                    return memory;
                }
                , memory => memory.Values.Sum())
            .AsResult1();

            foos.Aggregate(
                new Dictionary<long, long>(),
                (memory, group) =>
                {
                    group.Operations.ForEach(op => CalcPart2(memory, group.Mask, op));
                    return memory;
                }
                , memory => memory.Values.Sum())
            .AsResult2();


            Report.End();
        }

        private static void CalcPart1(Dictionary<long, long> memory, string mask, Operation o)
        {
            var sourceBinary = o.Source.ToBinary(mask.Length);
            sourceBinary = ApplyOverwriteMask(mask, sourceBinary);
            memory[o.Target] = sourceBinary.FromBinaryToLong();
        }
        private static void CalcPart2(Dictionary<long, long> memory, string mask, Operation o)
        {
            var targetBinary = o.Target.ToBinary(mask.Length);
            targetBinary = ApplySimpleMask(mask, targetBinary);
            foreach (var target in Flux(mask, targetBinary))
            {
                var t = target.FromBinaryToLong();
                memory[t] = o.Source;
            }
        }

        private static char[] ApplyOverwriteMask(string mask, char[] binary)
        {
            for (int i = 0; i < binary.Length; i++)
            {
                if (mask[i] == 'X') continue;

                binary[i] = mask[i];
            }
            return binary;
        }

        private static char[] ApplySimpleMask(string mask, char[] binary)
        {
            for (int i = 0; i < binary.Length; i++)
            {
                if (mask[i] == '1')
                {
                    binary[i] = '1';
                }
            }
            return binary;
        }

        private static IEnumerable<char[]> Flux(string mask, char[] binary, int start = 0)
        {
            var hasFlux = false;

            for (int i = start; i < binary.Length; i++)
            {
                if (mask[i] != 'X') continue;

                hasFlux = true;
                var fork = new[] { '0', '1' };

                var forks = fork
                    .SelectMany(f01 => Flux(mask, Fork(binary, i, f01), i + 1));

                foreach (var flux in forks)
                {
                    yield return flux;
                }
            }
            if (!hasFlux) yield return binary;
        }

        private static char[] Fork(char[] binary, int index, char val)
        {
            var fork = new char[binary.Length];
            binary.CopyTo(fork, 0);
            fork[index] = val;
            return fork;
        }

        public static List<OperationGroup> LoadFoos(string inputTxt, int top = 0)
        {
            var grps = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .GroupByLine(s => s.StartsWith("mask"), g => new OperationGroup()
                {
                    Mask = g.First().Splizz("=").Last(),
                    Operations = g.Skip(1).Select(s => ParseRegex(s)).ToList()
                });

            return grps.ToList();
        }

        private static Operation ParseRegex(string line)
        {
            Regex operationRegEx = new Regex(@"^mem\[(\d+)\] = (\d+)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);

            return new Operation()
            {
                Target = int.Parse(match.Groups[1].Value),
                Source = int.Parse(match.Groups[2].Value),
            };
        }
    }

    public static class Extensions
    {
        public static int ToInt(this char character)
        {
            return int.Parse(character.ToString());
        }
    }

    public static class StringExtensions
    {

        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
    }
}
