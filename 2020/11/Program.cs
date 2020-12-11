using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace aoc
{
    class Program
    {
        static void Main()
        {
            PlayGame(LoadPlane("input.txt"), (f, ff) => f.NextPart1Style(ff)).AsResult1();
            PlayGame(LoadPlane("input.txt"), (f, ff) => f.NextPart2Style(ff)).AsResult2();

            Report.End();
        }

        private static int PlayGame(List<PlaneSpace> planeSpaces, Func<PlaneSpace, Plane<PlaneSpace>, bool> calcNextState)
        {
            var plane = new Plane<PlaneSpace>(f => f.Pos);
            plane.Add(planeSpaces);

            var round = 0;
            while (true)
            {
                round++;
                var noChange = true;
                foreach (var f in plane.AllSpaces)
                {
                    if (calcNextState(f, plane))
                    {
                        noChange = false;
                    }
                }
                ApplyNewState(plane);
                if (noChange)
                {
                    round.Debug("Final Round");
                    return plane.AllSpaces.Count(s => s.CurrentState == PlaneSpace.OCCUPIED);
                }
            }
        }

        private static void ApplyNewState(Plane<PlaneSpace> plane)
        {
            foreach (var s in plane.AllSpaces)
            {
                s.CurrentState = s.NextState;
            }
        }

        public static List<PlaneSpace> LoadPlane(string inputTxt)
        {
            var planeSpaces = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany((row, y) => row.Select((planeSpace, x) => new PlaneSpace { Pos = new Point(x, y), CurrentState = planeSpace.ToString() }));
            var planeSpaceList = planeSpaces.ToList();
            Console.WriteLine($"Loaded {planeSpaces.Count()} entries ({inputTxt})");
            return planeSpaceList;
        }
    }
}
