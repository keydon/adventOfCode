using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextCopy;


namespace _04
{
    record PassProperty
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public static PassProperty Parse(string property){
            var kvp = property.Splizz(":").ToArray();
            return new PassProperty() {
                Type = kvp[0],
                Value = kvp[1]
            };
        }
    }
    record HeightProperty
    {
        public int Height { get; set; }
        public string Unit { get; set; }

    }

    class Program
    {
        private static string[] RelevantAndMandatoryProperties = new string[] {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};

        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var passports = LoadPassports("input.txt").ToList();

            var validCount1 = passports.Count(p => p.Count(s => s.Type != "cid") >= 7); 
            
            Console.WriteLine($"Part1-Result: {validCount1}");
            
            var validCount2 = passports
                .Select(c => c.Where(p => RelevantAndMandatoryProperties.Contains(p.Type)).ToList())
                .Select(p => ValidatePassport(p))
                .Count(p => p == true); 
            
            Console.WriteLine($"Part2-Result: {validCount2}");
            

            stopwatch.Stop();
            Console.WriteLine("\r\nCalculation took: {0}", stopwatch.Elapsed);
        }

        private static bool ValidatePassport(List<PassProperty> passwortProps)
        {
            if (passwortProps.Count < 7) 
                return false;

            return passwortProps.All(properties => ValidatePassportProperty(properties));
        }

        private static bool ValidatePassportProperty(PassProperty p)
        {
            switch (p.Type)
            {
                case "byr":
                    return ValidateNumberRange(p.Value, 1920, 2002);
                case "iyr":
                    return ValidateNumberRange(p.Value, 2010, 2020);
                case "eyr":
                    return ValidateNumberRange(p.Value, 2020, 2030);
                case "hgt":
                    return ValidateHeight(p);
                case "hcl":    
                    return RegexMatch(@"^#[0-9a-f]{6}$", p.Value);
                case "pid": 
                    return RegexMatch(@"^[0-9]{9}$", p.Value);
                case "ecl":
                    return ValidateEyeColor(p);
                default:
                    throw new Exception($"PassProperty not implemented yet: {p}");
            }
        }

        private static bool ValidateHeight(PassProperty p)
        {
            try
            {
                var hgt = ParseHeight(p.Value);
                if (hgt.Unit == "cm")
                {
                    return ValidateNumberRange(hgt.Height, 150, 193);
                }
                else if (hgt.Unit == "in")
                {
                    return ValidateNumberRange(hgt.Height, 59, 76);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidateEyeColor(PassProperty p)
        {
            var clrs = new string[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            return clrs.Contains(p.Value);
        }

        public static bool ValidateNumberRange(string numberString, int min, int max){
            if (!int.TryParse(numberString, out var number)) 
                return false;
            
            return ValidateNumberRange(number, min, max);
        }

        public static bool ValidateNumberRange(int number, int min, int max){            
            return min <= number && number <= max;
        }

        public static List<List<PassProperty>> LoadPassports(string inputTxt, int top = 0)
        {
            var passports = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .SelectMany(r => r.Splizz(" ").ToArray())
                .Aggregate(new LinkedList<List<PassProperty>>(), (passports, rawProperty) => {
                    if (rawProperty == "" || passports.Count==0) {
                        passports.AddLast(new List<PassProperty>());
                    }
                    
                    var currentPassport = passports.Last.Value;

                    if (rawProperty != "") {
                        currentPassport.Add(PassProperty.Parse(rawProperty));
                    }

                    return passports;
                });

            var foosList = passports.ToList();
            Console.WriteLine($"Loaded {passports.Count()} entries  in total ({inputTxt})");
            return foosList;
        }

        private static HeightProperty ParseHeight(string line)
        {
            Regex operationRegEx = new Regex(@"^(\d+)(.*)$");
            var match = operationRegEx.Match(line);
            if (!match.Success)
                throw new Exception("No RegEx-Match for " + line);

            return new HeightProperty()
            {
                Height = int.Parse(match.Groups[1].Value),
                Unit = match.Groups[2].Value
            };
        }

        private static bool RegexMatch(string expr, string line){
            Regex operationRegEx = new Regex(expr);
            var match = operationRegEx.Match(line);
            return match.Success;
        }
    }

    public static class Extensions
    {
        public static IEnumerable<string> Splizz(this string str, params string[] seps)
        {
            if (seps == null || seps.Length == 0)
                seps = new[] { ";", "," };
            return str.Split(seps, StringSplitOptions.TrimEntries).Select(e => e.Trim());
        }
    }    
}