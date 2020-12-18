using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Program
    {
        private static bool isPart2 = false;
        private static bool isVerbose = false;
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");

            isPart2 = false;
            foos.Select(s => new Stack<string>(s)).Select(s => Calc(s)).Sum().AsResult1();
            isPart2 = true;
            foos.Select(s => new Stack<string>(s)).Select(s => Calc(s)).Sum().AsResult2();


            Report.End();
        }

        private static long Calc(Stack<string> tokens, long? a = null)
        {
            bool isPrecedence = a.HasValue;
            string? op = null;
            long? b = null;
            while (tokens.TryPop(out var token))
            {
                if (long.TryParse(token, out long n))
                {
                    if (a.HasValue)
                        b = n;
                    else
                        a = n;
                }
                else if (token == "(")
                {
                    if (a.HasValue)
                        b = Calc(tokens);
                    else
                        a = Calc(tokens);
                }
                else if (token == ")")
                {
                    if (isPrecedence)
                        tokens.Push(")");
                    return a.Value;
                }
                else
                {
                    op = token;
                }

                if (isPart2 && b.HasValue && op == "*")
                {
                    b = Calc(tokens, b);
                }


                if (b.HasValue)
                {
                    if (op == "+")
                    {
                        var r = a + b;
                        if (isVerbose)
                            Console.WriteLine($"{a} + {b} = {r}");
                        a = r;
                        b = null;
                    }
                    else if (op == "*")
                    {
                        var r = a * b;
                        if (isVerbose)
                            Console.WriteLine($"{a} * {b} = {r}");
                        a = r;
                        b = null;
                    }
                }
            }
            return a.Value;
        }

        public static List<List<string>> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
             .Select(r => r.Splizz(" ", ";").SelectMany(s => ExpSplit(s)).Where(s => !string.IsNullOrWhiteSpace(s)).Reverse().ToList())
             .ToList();

            return foos;
        }

        private static IEnumerable<string> ExpSplit(string s)
        {
            var stack = new Stack<string>();
            foreach (var c in s)
            {
                if (c == '(' && stack.Count > 0)
                {
                    yield return stack.Select(s => s).Reverse().ToCommaString("");
                    stack.Clear();
                    yield return c.ToString();
                }
                else if (c == '(')
                {
                    yield return c.ToString();
                }
                else if (c == ')')
                {
                    yield return stack.Select(s => s).Reverse().ToCommaString("");
                    stack.Clear();
                    yield return c.ToString();
                }
                else
                {
                    stack.Push(c.ToString());
                }
            }
            if (stack.Count > 0)
            {
                yield return stack.Select(s => s).Reverse().ToCommaString("");
                stack.Clear();
            }
        }
    }
}
