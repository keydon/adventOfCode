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


namespace _01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var groups = LoadGroupAnswers("input.txt");

            var result1 = groups
                .Select(g => g.SelectMany(p => p).Distinct())
                .Sum(g => g.Count());

            Console.WriteLine($"Part1-Result: {result1}");


            // First working solution of part II
            /* int sum = 0;
             foreach (var foo in groups)
             {
                 var res = foo.SelectMany(g => g).Distinct().ToList();

                 foreach (var g in foo)
                 {
                     res = res.Intersect(g).ToList();
                 }
                 sum += res.Count();
             }*/

            var result2 = groups
                .Select(g => new
                {
                    allAnswers = g.SelectMany(a => a),
                    Group = g
                })
                .Select(g => g.Group.Aggregate(g.allAnswers, (unanimous, answers) => unanimous.Intersect(answers)))
                .Sum(g => g.Count());

            Console.WriteLine($"Part2-Result: {result2}");
        }

        public static List<List<List<char>>> LoadGroupAnswers(string inputTxt, int top = 0)
        {
            const string GROUP_SEPERATOR = "";
            var groups = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .Aggregate(new LinkedList<List<List<char>>>(), (groups, personsQuestions) =>
                {
                    if (personsQuestions == GROUP_SEPERATOR || groups.Count == 0)
                    {
                        groups.AddLast(new List<List<char>>());

                    }

                    var group = groups.Last.Value;

                    if (personsQuestions != GROUP_SEPERATOR)
                    {
                        var questions = personsQuestions.Select(c => c).ToList();
                        group.Add(questions);
                    }

                    return groups;
                });
            ;

            var groupsList = groups.ToList();
            Console.WriteLine($"Loaded {groups.Count()} entries ({inputTxt})");
            return groupsList;
        }
    }
}
