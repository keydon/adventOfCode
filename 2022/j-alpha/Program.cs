using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true){
                var rnd = new Random();
                var left = rnd.Next((int)'a'+15, (int)'z'-3);
                var right = left + rnd.Next(-3,3);

                if(left == right)
                    continue;

                Console.WriteLine("Welcher Buchstabe kommt zuerst? {0} oder {1}?", (Char)left, (Char)right);
                var user = Console.ReadKey().KeyChar;
                var first = Math.Min(left, right);
                if (user == first){
                    Console.WriteLine("\r\nRichtig!");
                } else {
                    Console.WriteLine("\r\nLeider nein, {0} kommt vor {1}", (char)Math.Min(left, right), (char)Math.Max(left, right));
                }
                Console.WriteLine("");
            }
        }

        public static List<string> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())


             //.GroupByLineSeperator()
             
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
             //      A = m.Groups[1].Value,
             //      B = m.Groups[2].Value,
             //  }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
             .ToList()
            ;

            return foos;
        }
    }
}
