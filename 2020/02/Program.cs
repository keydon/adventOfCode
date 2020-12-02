using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _02
{
    record PasswordPolicy
    {
        public int Low { get; set; }
        public int High { get; set; }
        public char Letter { get; set; }
        public string Password { get; set; }

        public bool isValid1(){
            int occurences = 0;
            foreach (var l in Password)
            {
                if (l == Letter) {
                    occurences++;
                }
            }
            return Low<= occurences && occurences <= High;
        }
        
        public bool isValid2(){
            var pos1 = Low - 1;
            var pos2 = High - 1;
            bool ok1 = Password[pos1] == Letter;
            bool ok2 = Password[pos2] == Letter;
            
            return ok1 ^ ok2;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var foos = LoadFoos("input.txt");

            Console.WriteLine("==== Part 1 ====");
            var result1 = foos.Count(f => f.isValid1());
            Console.WriteLine($"Part1-Result: {result1}");

      
            Console.WriteLine("==== Part 2 =====");
            var result2 = foos.Count(f => f.isValid2());
            Console.WriteLine($"Part2-Result: {result2}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static List<PasswordPolicy> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Select(r => r.Splizz(",", ";", "-", ":", " ").ToArray())
                .Select(s => new PasswordPolicy() { 
                    Low = int.Parse(s[0]), 
                    High = int.Parse(s[1]),
                    Letter= s[2][0],
                    Password=s[3]
                })
                .ToList();
            return foos;
        }
    }
    public static class Extensions
    {
        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
    }
}
