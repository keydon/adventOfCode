using System;
using System.Drawing;
using System.Linq;
using day10;
using Xunit;

namespace day08.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Sample01()
        {
            var loadAsteroids = Program.LoadAsteroids("sample01.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var bestExposure = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();
            Assert.Equal(new Point(3,4), bestExposure.Point);
        }

        [Fact]
        public void Sample02()
        {
            var loadAsteroids = Program.LoadAsteroids("sample02.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var bestExposure = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();

            //Console.WriteLine(string.Join("\r\n", bestExposure.Debug));

            Assert.Equal(new Point(5,8), bestExposure.Point);
            Assert.Equal(33, bestExposure.Visible.Count);
        }

        [Fact]
        public void Sample03()
        {
            var loadAsteroids = Program.LoadAsteroids("sample03.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var bestExposure = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();

            //Console.WriteLine(string.Join("\r\n", bestExposure.Debug));

            Assert.Equal(new Point(1,2), bestExposure.Point);
            Assert.Equal(35, bestExposure.Visible.Count);
        }

        [Fact]
        public void Sample04()
        {
            var loadAsteroids = Program.LoadAsteroids("sample04.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var bestExposure = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();

            //Console.WriteLine(string.Join("\r\n", bestExposure.Debug));

            Assert.Equal(new Point(6,3), bestExposure.Point);
            Assert.Equal(41, bestExposure.Visible.Count);
        }
        [Fact]
        public void Sample05()
        {
            var loadAsteroids = Program.LoadAsteroids("sample05.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var bestExposure = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();

            //Console.WriteLine(string.Join("\r\n", bestExposure.Debug));

            Assert.Equal(new Point(11,13), bestExposure.Point);
            Assert.Equal(210, bestExposure.Visible.Count);
        }

        [Fact]
        public void DegreeDiagonal()
        {
            var loadAsteroids = Program.LoadAsteroids("laserSample.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var station = loadAsteroids.First().AstroidsDic[new Point(8, 3)];
            var calculated = loadAsteroids.Select(a => Program.CalcDegree(station, a)).ToList();
            //Console.WriteLine(string.Join(",\r\n", calculated.OrderByDescending(a => a.Degree).Select(a => $"{a.Degree} {a.Point}")));
        }

        [Fact]
        public void DegreeDiagonal05()
        {
            var loadAsteroids = Program.LoadAsteroids("live.txt");
            Program.CalculateAllLinesOfSight(loadAsteroids);
            var station = loadAsteroids.OrderByDescending(a => a.Visible.Count).First();
            var calculated = loadAsteroids
                .Select(a => Program.CalcDegree(station, a))
                .ToList();


            var victimCounter = 0;

            while (loadAsteroids.Count > 1)
            {
                var rotationVisible = calculated
                    .Where(c => station.Visible.Contains(c.Point))
                    .OrderByDescending(a => a.Degree)
                    .ToList();
                if (rotationVisible.Count == 0)
                {
                    Console.WriteLine($"Remaining astroids: {string.Join(",", loadAsteroids.Select(a => a.Point))}");
                    Console.WriteLine($"Visible astroids: {string.Join(",", station.Visible)}");
                    Console.WriteLine($"blocked astroids: {string.Join(",", station.Blocked)}");
                    Console.WriteLine($"candis astroids: {string.Join(",", station.Candidates)}");
                    throw new Exception("wtf");
                }

                foreach (var astroid in rotationVisible)
                {
                    Console.WriteLine($"{++victimCounter}. {astroid.Point} vaporized!");
                }

                loadAsteroids = loadAsteroids.Except(rotationVisible).ToList();
                if(!loadAsteroids.Contains(station)) throw new Exception("Oh oh, we nuked ourselfs!");
                var points = loadAsteroids.Select(ar => ar.Point).ToList();
                var dic = loadAsteroids.ToDictionary(
                    (a) => a.Point,
                    (a) => a
                );

                loadAsteroids.ForEach(a =>
                {
                    a.SetCandidates(points);
                    a.SetAstroidDictionary(dic);
                });
                Program.CalculateAllLinesOfSight(loadAsteroids);

            }
            Console.WriteLine("DONE!");

            //Console.WriteLine(string.Join(",\r\n", calculated.OrderByDescending(a => a.Degree).Select(a => $"{a.Degree} {a.Point}")));
        }
    }
}
