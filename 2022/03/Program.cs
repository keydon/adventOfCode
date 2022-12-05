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
            var elfs = LoadElfs("input.txt");
            //elfs = LoadElfs("sample.txt");

            var priomap = CreatePrioMap();

            elfs.Select(rucksack => rucksack.SelectStrings().ToList())
                .Select(rucksackItems => new
                {
                    FirstCompartment = rucksackItems.Take(rucksackItems.Count / 2),
                    SecondCompartment = rucksackItems.Skip(rucksackItems.Count / 2),
                }).Select(rucksack => rucksack.FirstCompartment.Intersect(rucksack.SecondCompartment))
                .SelectMany(CommonItems => CommonItems.Select(item => priomap[item]))
                .Sum()
                .AsResult1();

            elfs.Select((rucksack, i) => new
                {
                    Group = Math.Ceiling((double)(i+1) / 3),
                    Items = rucksack.SelectStrings(),
                })
                .GroupBy(group => group.Group, v => v)
                .Select(group => group.Select(elf => elf.Items).IntersectMany().Single())
                .Select(commonItem => priomap[commonItem])
                .Sum()
                .AsResult2();

            Report.End();
        }

        private static Dictionary<string, int> CreatePrioMap()
        {
            var lower = Enumerable.Range('a', 'z' - 'a' + 1);
            var upper = Enumerable.Range('A', 'Z' - 'A' + 1);
            return lower.Concat(upper)            
                .Select((charAsInt, i) => new { Letter = ((char)charAsInt).ToString(), Priority = i + 1 })
                .ToDictionary(key => key.Letter, value => value.Priority);
        }

        public static List<string> LoadElfs(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
