using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextCopy;

namespace aoc
{
    record Pointish
    {
        public Direction Facing { get; set; }
        public Point Pos { get; set; }

        public void Move(Direction direction, int steps)
        {
            Pos = Pos.Move(direction, steps);
        }
        public void Move(Point waypoint, int steps)
        {
            Pos = Pos.Move(waypoint, steps);
        }
        public void RotateFacing(Rotation rotation)
        {
            Facing = Facing.Rotate(rotation);
        }
        public void RotateOrientation(Rotation rotation)
        {
            Pos = Pos.Rotate(rotation);
        }
    }
    record Movement
    {
        public int Amount { get; set; }
        public string Act { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var moves = LoadMovements("input.txt");
            var ship1 = new Pointish() { Facing = Direction.RIGHT };
            foreach (var move in moves)
            {
                MovePart1Style(ship1, move);

            }
            ship1.Pos.ManhattenDistance().AsResult1();

            var ship2 = new Pointish();
            var waypoint = new Pointish() { Pos = new Point(0, 0).Right(10).Up(1) };
            foreach (var move in moves)
            {
                MovePart2Style(ship2, move, waypoint);

            }
            ship2.Pos.ManhattenDistance().AsResult2();
            Report.End();
        }

        private static void MovePart2Style(Pointish ship, Movement move, Pointish waypoint)
        {
            switch (move.Act)
            {
                case "F":
                    ship.Move(waypoint.Pos, move.Amount);
                    break;
                case "E":
                case "N":
                case "W":
                case "S":
                    var direction = PointParseUtils.ParseDirection(move.Act);
                    waypoint.Move(direction, move.Amount);
                    break;
                case "L":
                case "R":
                    var rotation = PointParseUtils.ParseRotation(move.Act, move.Amount);
                    waypoint.RotateOrientation(rotation);
                    break;
                default:
                    throw new Exception("Unknown act: " + move.Act);
            }
        }

        private static void MovePart1Style(Pointish ship, Movement move)
        {
            switch (move.Act)
            {
                case "F":
                    ship.Move(ship.Facing, move.Amount);
                    break;
                case "E":
                case "N":
                case "W":
                case "S":
                    var direction = PointParseUtils.ParseDirection(move.Act);
                    ship.Move(direction, move.Amount);
                    break;
                case "L":
                case "R":
                    var rotation = PointParseUtils.ParseRotation(move.Act, move.Amount);
                    ship.RotateFacing(rotation);
                    break;
                default:
                    throw new Exception("Act not impl: " + move.Act);
            }
        }

        public static List<Movement> LoadMovements(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => ParseRegex(s));

            var foosList = foos.ToList();
            Console.WriteLine($"Loaded {foos.Count()} entries ({inputTxt})");
            return foosList;
        }

        private static Movement ParseRegex(string line)
        {
            Regex operationRegEx = new Regex(@"^([A-Z])(\d+)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for: " + line);

            return new Movement()
            {
                Amount = int.Parse(match.Groups[2].Value),
                Act = match.Groups[1].Value,
            };
        }
    }
}
