using System;
using System.Diagnostics;

namespace aoc
{
    public static class Report
    {
        private static Stopwatch stopwatch;
        private static object result1;
        private static bool result1Called = false;

        private static string copyInfo;
        private static string lastResult;
        public static T PrintResult1<T>(T result)
        {
            var bannerCharacter = '=';
            if (result1Called)
            {
                bannerCharacter = '!';
            }
            result1Called = true;
            result1 = result;
            PrintBanner(result, "1", bannerCharacter);
            return result;
        }
        public static T PrintResult2<T>(T result = default)
        {
            if (object.Equals(result, default(T)))
            {
                Console.WriteLine($"Result 2 is fishy! {result}");
                return result;
            }
            var bannerCharacter = '=';
            if (result.Equals(result1))
            {
                bannerCharacter = '?';
            }
            result1 = result;
            PrintBanner(result, "2", bannerCharacter);
            return result;
        }

        private static void PrintBanner(object result, string part, char bannerCharacter = '=')
        {
            var headline = $" RESULT {part} ";
            Console.WriteLine(new string(bannerCharacter, 10) + headline + new string(bannerCharacter, 10));
            Console.Write(new string(bannerCharacter, 9) + $">> {result} <<" + new string(bannerCharacter, 9));
            stopwatch.Stop();
            Console.WriteLine($" Calculation took: {stopwatch.Elapsed} " + new string(bannerCharacter, 3));
            stopwatch.Restart();
            Console.WriteLine(new string(bannerCharacter, 30));

            if (bannerCharacter == '=')
            {
                copyInfo = "Part " + part;
                lastResult = $"{result}";
                TextCopy.ClipboardService.SetText(lastResult);
            }
        }

        internal static void Start()
        {
            stopwatch = Stopwatch.StartNew();
        }

        internal static void End()
        {
            Console.WriteLine("END. Clipbord: " + copyInfo + ": " + lastResult);
        }

        public static T AsResult1<T>(this T result)
        {
            return PrintResult1(result);
        }

        public static T AsResult2<T>(this T result)
        {
            return PrintResult2(result);
        }
    }
}