using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    public record Cuboid
    {
        public string PowerState { get; set; }
        public int LowerX { get; internal set; }
        public int UpperX { get; internal set; }
        public int LowerY { get; internal set; }
        public int UpperY { get; internal set; }
        public int LowerZ { get; internal set; }
        public int UpperZ { get; internal set; }
        public decimal Volume { get {
            long sign = PowerState == "on" ? 1 : -1;
            return sign * (UpperX - LowerX + 1) * (UpperY - LowerY + 1) * (UpperZ - LowerZ + 1);
        } }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();

            var cuboids = LoadCuboids("input.txt");
            // cuboids = LoadCuboids("sample.txt"); 

            var initArea = cuboids.Where(c => InArea(c, -50, 50));
            Boot(initArea).Sum(b => b.Volume).AsResult1();
            Boot(cuboids).Sum(b => b.Volume).AsResult2();

            Report.End();
        }

        private static List<Cuboid> Boot(IEnumerable<Cuboid> bootSequence){
            var normalizedSequence = new List<Cuboid>();
            foreach (var procedure in bootSequence)
            {
                normalizedSequence.AddRange(Program.Normalize(normalizedSequence, procedure).ToList());
            }
            return normalizedSequence;
        }

        private static bool InArea(Cuboid f, int lowerBound, int upperBound)
        {   
            var lows = new [] {f.LowerX, f.LowerY, f.LowerZ};
            var highs = new [] {f.UpperX, f.UpperY, f.UpperZ};
            var lowest = lows.Min();
            var highest = highs.Max();

            if(lowest < lowerBound)
                return false;
            if(highest > upperBound)
                return false;
            return true;
        }

        public static IEnumerable<Cuboid> Normalize(List<Cuboid> booted, Cuboid cube)
        {
            if(cube.PowerState == "on"){
                yield return cube;
            }
            foreach (var booting in booted)
            {
                var intersect = Intersect(booting, cube);
                if (IsValid(intersect)) {
                    yield return intersect;
                }
            }
        }

        private static bool IsValid(Cuboid foo)
        {
            if(foo.LowerX > foo.UpperX)
                return false;
            if(foo.LowerY > foo.UpperY)
                return false;
            if(foo.LowerZ > foo.UpperZ)
                return false;
            return true;
        }

        private static string CalcPowerState(string booted, string action)
        {
            return (booted, action) switch
            {
                ("on", "on") => "off",
                ("on", "off") => "off",
                ("off", "off") => "on",
                ("off", "on") => "on",
                _ => throw new Exception($"Illegal State change {booted} -> {action}")
            };
        }

        private static Cuboid Intersect(Cuboid booted, Cuboid instruction)
        {
           var cubes = new[]{booted, instruction};
           return new Cuboid(){ 
               LowerX = cubes.Max(f => f.LowerX), 
               UpperX = cubes.Min(f => f.UpperX), 
               LowerY = cubes.Max(f => f.LowerY), 
               UpperY = cubes.Min(f => f.UpperY), 
               LowerZ = cubes.Max(f => f.LowerZ), 
               UpperZ = cubes.Min(f => f.UpperZ),
               PowerState = CalcPowerState(booted.PowerState, instruction.PowerState)
            };          
        }

        public static List<Cuboid> LoadCuboids(string inputTxt)
        {
            var cuboids = File
                .ReadAllLines(inputTxt.Debug("input"))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^(on|off) x=([-0-9]+)..([-0-9]+),y=([-0-9]+)..([-0-9]+),z=([-0-9]+)..([-0-9]+)$", m => new Cuboid()
                {
                   PowerState = m.Groups[1].Value,
                   LowerX = int.Parse(m.Groups[2].Value),
                   UpperX = int.Parse(m.Groups[3].Value),
                   LowerY = int.Parse(m.Groups[4].Value),
                   UpperY = int.Parse(m.Groups[5].Value),
                   LowerZ = int.Parse(m.Groups[6].Value),
                   UpperZ = int.Parse(m.Groups[7].Value),
                })).ToList();

            return cuboids;
        }
    }
}
