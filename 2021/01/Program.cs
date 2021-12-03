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
            Report.Start();
            var measurements = LoadMeasurements("input.txt");

            CountIncreases(measurements).AsResult1();

            List<long> improvedMeasurements = ReduceNoise(measurements);
            CountIncreases(improvedMeasurements).AsResult2();

            Report.End();
        }

        private static List<long> ReduceNoise(List<long> measurements)
        {
            var threeSlidingWindow = new List<long>();

            for (int i = 0; i < measurements.Count - 2; i++)
            {
                threeSlidingWindow.Add(measurements[i] + measurements[i + 1] + measurements[i + 2]);
            }

            return threeSlidingWindow;
        }

        private static int CountIncreases(List<long> measurments)
        {
            var lastDepth = 0L;
            var count = -1;
            foreach (var currentDepth in measurments){
                if (currentDepth > lastDepth)
                    count++;
                
                lastDepth = currentDepth;
            }
            return count;
        }

        public static List<long> LoadMeasurements(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(long.Parse)  
                .ToList();

            return foos;
        }
    }
}
