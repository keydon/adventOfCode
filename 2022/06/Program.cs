using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
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
            var packets = LoadSignal("input.txt").ToList();
            //packets = LoadSignal("sample.txt");

            DetectMarker(packets, 4).AsResult1();
            
            DetectMarker(packets, 14).AsResult2();

            Report.End();
        }

        private static int DetectMarker(List<string> packets, int sequenceLength)
        {
            for (int i = (sequenceLength-1); i < packets.Count; i++)
            {
                var sequence = Enumerable.Range(0, sequenceLength)
                    .Select(d => packets[i - d])
                    .ToHashSet();
                if (sequence.Count == sequenceLength)
                {
                    return i + 1;
                }
            }
            throw new Exception("Marker not found");
        }

        public static List<string> LoadSignal(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany(c => c.ToCharArray().Select(a => a.ToString()))
                .ToList();
        }
    }
}
