using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record ElfPair 
    {
        public Range Elf1 { get; set; }
        public Range Elf2 { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            foos.Where(pair => pair.Elf1.FullyContains(pair.Elf2) || pair.Elf2.FullyContains(pair.Elf1))
                .Count()
                .AsResult1();

            foos.Where(pair => pair.Elf1.IsOverlaping(pair.Elf2))
                .Count().AsResult2();

            Report.End();
        }

        public static List<ElfPair> LoadFoos(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^(\d+)-(\d+),(\d+)-(\d+)$", m => new ElfPair()
                {
                        Elf1 = new Range(){ Start = int.Parse(m.Groups[1].Value), End = int.Parse(m.Groups[2].Value) },
                        Elf2 = new Range(){ Start = int.Parse(m.Groups[3].Value), End = int.Parse(m.Groups[4].Value) },
                }))
                .ToList();
        }
    }
}
