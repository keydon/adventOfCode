using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Race 
    {
        public long Time { get; set; }
        public long Distance { get; set; }

        internal IEnumerable<Race> GeneratePossibleCombinations()
        {
            return LongRange.Generate(0, Time)
                .Select(t => Race.WithTimes(t, Time -t))
                .Where(r => r.Distance > Distance);
        }

        static Race WithTimes(long charging, long moving){
            var speed = charging * 1;
            return new Race(){
                Time = charging + moving,
                Distance = speed * moving
            };
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            
            var racesI = LoadRacesPartI("input.txt");
            racesI.Select(
                    r => r.GeneratePossibleCombinations().Count()
                )
                .MultiplyAll()
                .AsResult1();

            var racesII = LoadRacesPartII("input.txt");
            racesII.Select(
                    r => r.GeneratePossibleCombinations().Count()
                )
                .MultiplyAll()
                .AsResult2();

            Report.End();
        }

        public static IEnumerable<Race> LoadRacesPartI(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim());

            var times = foos.First().Splizz(":", " ").Skip(1).ToList();
            var distances = foos.Last().Splizz(":", " ").Skip(1).ToList();
            var races = times.Zip(distances)
              .Select((z, i) => new Race() {
                Time = long.Parse(z.First),
                Distance = long.Parse(z.Second)
            });
            return races;
        }
        public static IEnumerable<Race> LoadRacesPartII(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim());

            var times = foos.First().Splizz(":").Skip(1).ToList();
            var distances = foos.Last().Splizz(":").Skip(1).ToList();
            var races = times.Zip(distances)
              .Select((z, i) => new Race() {
                Time = long.Parse(z.First.Replace(" ", "")),
                Distance = long.Parse(z.Second.Replace(" ", "")),
            });
            return races;
        }
    }
}