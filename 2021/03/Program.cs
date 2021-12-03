using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    enum Bit {
        b0,
        b1,
    }
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var data = LoadDiagnosticReport("input.txt");

            var gamma = process(data, 
                    (count0, count1) => count0 > count1 ? '0' : '1')
                .FromBinaryToLong().Debug("Gamma");

            var epsilon = process(data, 
                    (count0, count1) => count0 < count1 ? '0' : '1')
                .FromBinaryToLong().Debug("Epsilon");

            var powerConsumption = (gamma * epsilon).AsResult1();
            
            
            var oxyRating = process(data, 
                    (count0, count1) => count0 > count1 ? '0' : '1',
                    (bit, data) => data.Where(s => s[0] == bit))
                .FromBinaryToLong().Debug("Oxygen generator rating");

            var scrubberRating = process(data, 
                    (count0, count1) => count1 < count0 ? '1' : '0',
                    (bit, data) => data.Where(s => s[0] == bit))
                .FromBinaryToLong().Debug("CO2 scrubber rating");
            
            var lifeSupportRating = (oxyRating * scrubberRating).AsResult2();

            Report.End();
        }

        private static char FindBit(List<string> data, Func<int, int, char> finder){
            var nulls = data.Count(s => s[0] == '0');
            var ones = data.Count - nulls;
            return finder(nulls, ones);
        }

        private static string process(List<string> data, Func<int, int, char> finder, Func<char, IEnumerable<string>,IEnumerable<string>> filter = null){
            if (data == null || data.Count == 0) 
                return string.Empty;
            if (data.Count == 1){
                return data.First();
            }
        
            var bit = FindBit(data, finder);
            var newData = (filter == null ? data : filter(bit, data))
                .Select(s => s.Substring(1)).Where(s => s.Length > 0).ToList();
            
            var bits = process(newData, finder, filter);
            return bit + bits;
        }

        public static List<string> LoadDiagnosticReport(string inputTxt)
        {
            var report = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            return report;
        }
    }
}
