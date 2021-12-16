using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Octopus : IHasPosition<Point2>
    {
        public static int FlashCount = 0;

        public int Energy { get; set; }
        public bool Flashed { get; set; }

        public Point2 Pos { get; set; }

        public bool NeedsToFlash()
        {
            return Energy > 9 && !Flashed;
        }
        public void Flash(){
            Flashed = true;
            FlashCount++;
        }
        public void ResetAfterFlash(){
            if (Flashed){
                Flashed = false;
                Energy = 0;
            }
        }

        
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var octopuses = LoadOctopuses("input.txt");
            // octopuses = LoadOctopuses("sample2.txt");
            
            var field = new Field<Point2, Octopus>(OutOfBoundsStrategy.RETURN_NULL);
            field.Add(octopuses);
            
            var ttl = 2;
            var step = 0;
            while (ttl > 0) {
                step++;
                field.AllFields.ForEach(f => f.Energy++);

                while (true) {
                    var needToFlash = field.AllFields.Where(f => f.NeedsToFlash()).ToList();
                    if (needToFlash.Count == 0)
                        break;
                    needToFlash.ForEach(oct => oct.Flash());
                    needToFlash.SelectMany(oct => field.GetNeighbours(oct))
                        .Where(n => !n.Flashed)
                        .ForEach(n => n.Energy++);
                }
                
                if (field.AllFields.All(f => f.Flashed)){
                    step.AsResult2();
                    ttl--;
                }
                if (step == 100) {
                    Octopus.FlashCount.AsResult1();
                    ttl--;
                }

                field.AllFields.ForEach(o => o.ResetAfterFlash());
            }

            Report.End();
        }

        public static List<Octopus> LoadOctopuses(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Parse2DMap((p, t) => new Octopus { Pos = p, Energy = int.Parse(t) })
                .ToList();

            return foos;
        }
    }
}
