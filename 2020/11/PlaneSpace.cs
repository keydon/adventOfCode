using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace aoc
{
    record PlaneSpace
    {
        public const string OCCUPIED = "#";
        public const string EMPTYSEAT = "L";
        public const string FLOOR = ".";
        public string CurrentState { get; set; }
        public string NextState { get; set; }

        public Point Pos { get; set; }

        public bool NextPart1Style(Plane<PlaneSpace> field)
        {
            var foos = field.GetNeighbours(this);

            var occupancyCount = foos.Count(f => f.IsOccupied());
            if (occupancyCount == 0 && IsSeatEmpty())
            {
                NextState = OCCUPIED;
                return true;
            }
            else if (occupancyCount >= 4 && IsOccupied())
            {
                NextState = EMPTYSEAT;
                return true;
            }
            else
            {
                NextState = CurrentState;
                return false;
            }
        }
        public bool NextPart2Style(Plane<PlaneSpace> field)
        {
            var directions = new Func<PlaneSpace, Point>[]{
                s => s.Pos.Up(),
                s => s.Pos.Up().Right(),
                s => s.Pos.Up().Left(),
                s => s.Pos.Down(),
                s => s.Pos.Down().Left(),
                s => s.Pos.Down().Right(),
                s => s.Pos.Left(),
                s => s.Pos.Right()
            };

            var neighbours = directions
                .Select(d => MoveWhile(d, f => f.IsFloor(), field, this))
                .Where(f => f != null);

            var occupancyCount = neighbours.Count(f => f.IsOccupied());
            if (occupancyCount == 0 && IsSeatEmpty())
            {
                NextState = OCCUPIED;
                return true;
            }
            else if (occupancyCount >= 5 && IsOccupied())
            {
                NextState = EMPTYSEAT;
                return true;
            }
            else
            {
                NextState = CurrentState;
                return false;
            }
        }

        public bool IsOccupied()
        {
            return CurrentState == OCCUPIED;
        }
        public bool IsSeatEmpty()
        {
            return CurrentState == EMPTYSEAT;
        }
        public bool IsFloor()
        {
            return CurrentState == FLOOR;
        }

        internal static IEnumerable<PlaneSpace> Move(Func<PlaneSpace, Point> direction, Plane<PlaneSpace> plane, PlaneSpace s)
        {
            while (true)
            {
                var next = direction(s);
                var nextFoo = plane.GetDef(next);
                if (nextFoo == null)
                {
                    yield break;
                }
                yield return nextFoo;
                s = nextFoo;
            }
        }

        internal static PlaneSpace MoveWhile(Func<PlaneSpace, Point> direction, Func<PlaneSpace, bool> moveWhile, Plane<PlaneSpace> plane, PlaneSpace s)
        {
            return Move(direction, plane, s).SkipWhile(moveWhile).FirstOrDefault();
        }

    }
}
