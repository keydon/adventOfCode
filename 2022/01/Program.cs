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

      var elfs = LoadElfs("input.txt")
        .Select((elf, index) =>
        new {
          No = index + 1,
          TotalCalories = elf.Select(item => long.Parse(item)).Sum(),
        })
        .ToList();

      var richestElf = elfs
        .OrderByDescending(elf => elf.TotalCalories)
        .First()
        .AsResult1();

      var top3 = elfs
        .OrderByDescending(elf => elf.TotalCalories)
        .Take(3)
        .Sum(elf => elf.TotalCalories)
        .AsResult2();

      Report.End();
    }

    public static List<List<string>> LoadElfs(string inputTxt)
    {
      return File
        .ReadAllLines(inputTxt)
        .Select(s => s.Trim())
        .GroupByLineSeperator()
        .ToList();
    }
  }
}
