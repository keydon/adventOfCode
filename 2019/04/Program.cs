using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace day04
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
      
            var possiblePasswords = Enumerable.Range(109165,  576723-109165)
                    .AsParallel()
                    .Where(isNotDecreasing)
                    .Where(hasSiblings)
                    .ToList();

            Console.WriteLine(">> Possible Passwords: {0} <<", possiblePasswords.Count);
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            
            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();  

            var possiblePasswordsPart2 = possiblePasswords
                    .AsParallel()
                    .Where(hasPairs)
                    .ToList();  
                
            Console.WriteLine(">> Possible Passwords: {0} <<", possiblePasswordsPart2.Count);
            stopwatch.Stop();
            Console.WriteLine("Calculation took : {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static bool hasSiblings(int arg)
        {
            var str = arg.ToString();
            for (int i = 0; i < str.Length-1; i++)
            {
                var a = str[i];
                var b = str[i+1];

                if(b == a) {
                    return true;
                }
            }
            return false;
        }

        private static bool hasPairs(int arg)
        {
            var str = arg.ToString();
            for (int i = 0; i < str.Length-1; i++)
            {
                var a = str[i];
                var b = str[i+1];

                if(b == a) {
                    var preA = i>0 ? str[i-1] : 'x';
                    var afterB = i+1 < str.Length-1 ? str[i+2] : 'x';
                    if(a != preA && b != afterB){
                        return true;
                    }
                }
                    
            }
            return false;
        }

        private static bool isNotDecreasing(int arg)
        {
            var str = arg.ToString();
            for (int i = 0; i < str.Length-1; i++)
            {
                var a = str[i];
                var b = str[i+1];

                if(b < a)
                    return false;
            }
            return true;
        }
    }

    public static class Extensions
    {
        public static IEnumerable<string> Splizz(this string str, params string[] seps){
            if(seps == null || seps.Length == 0)
                seps = new[]{";", ","};
            return str.Split(seps,StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
    }
}
