using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;


namespace _03
{
    record Field
    {
        public string Type { get; set; }
        public Point Position {get; set; }

        public bool IsTree(){
            return Type == "#";
        }
    }
    class Program
    {
        private static Dictionary<Point, Field> fieldLookup;
        private static int maxX;
        private static int maxY;

        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 & 2 ====");
            var foos = LoadMap("input.txt");
            maxX = foos.Max(f => f.Position.X);
            maxY = foos.Max(f => f.Position.Y);
            fieldLookup = foos.ToDictionary(f => f.Position);
            Console.WriteLine($"maxX: {maxX}, maxY: {maxY}");      

            var slopes = new Point[] {new Point(1,1), new Point(3, 1), new Point(5, 1), new Point(7, 1), new Point(1,2)}; 

            var result1 = slopes
                .Select(s => CountTrees(s))
                .Peak()
                .Aggregate(1, (a,b) => a * b);
           
            Console.WriteLine($"Part2-Result: {result1}");
        }

        static int CountTrees(Point slope){
            return Traverse(slope).Count(f => f.IsTree());
        }

        static IEnumerable<Field> Traverse(Point slope){
            var field = fieldLookup[new Point(0,0)];
            
            while (true) {
                yield return field;
                field = MoveBySlope(field.Position, slope);
                if (field == null) {
                    break; // reached bottom
                }
            }
        }

        static Field MoveBySlope(Point pos, Point slope) {
            var newPos = new Point(pos.X + slope.X, pos.Y + slope.Y);
            if(newPos.Y > maxY) {
                return null;
            }
            if(newPos.X > maxX) {
                // hups, out bounds. just start from the left again
                newPos = new Point(newPos.X - maxX - 1, newPos.Y);
            }
            return fieldLookup[newPos];
        }

        public static List<Field> LoadMap(string inputTxt, int top = 0)
        {
            var fields = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany((row, y) => row.Select((fieldType, x) => new Field { Position = new Point(x, y), Type = fieldType.ToString() }));

            var fieldsList = fields.ToList();
            Console.WriteLine($"Loaded {fields.Count()} entries ({inputTxt})");
            return fieldsList;
        }
    }
    public static class Extensions
    {
        public static IEnumerable<T> Peak<T>(this IEnumerable<T> enumerable, string prefix = ""){
            return enumerable.Select(x => {
                Console.WriteLine($"Peak: {prefix} {x}");
                return x;
            });
        }
    }
}
