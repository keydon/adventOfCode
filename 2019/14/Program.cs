using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace day14
{
    class Program
    {
        private const string input = "input.txt";

        public static Dictionary<string, Recepie> dic = new Dictionary<string, Recepie>(StringComparer.InvariantCultureIgnoreCase);
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
      
            dic = File.ReadAllLines(input)
                .Select(ParseRecepie)
                .ToDictionary(r => r.Name, r => r);

            dic.Add("ORE", new Recepie(){Name = "ORE", Amount = 1});

            //dic["FUEL"].Dump();
            Console.WriteLine(">> Possible Passwords: {0} <<", dic["FUEL"].Cost());
            stopwatch.Stop();
            Console.WriteLine("Execution took: {0}", stopwatch.Elapsed);
        }

        private static Recepie ParseRecepie(string line)
        {
            List<string> list = line.Splizz(" => ").ToList();
            var components = list.First();
            var result = list
                .Skip(1)
                .RegExParse<Recepie>(@"^(?<Amount>[0-9]+) (?<Name>.+)$")
                .First();

            result.Components = components.Splizz(", ")
                    .RegExParse<Recepie>(@"^(?<Amount>[0-9]+) (?<Name>.+)$")
                    .ToList();
            return result;
        }

    }

    public class Recepie
    {
        public Recepie(){
            Components = new List<Recepie>();
        }
        public string Name { get; set; }
        public int Amount {get; set;}
        public int TotalAmount {get; set;}
        
        public List<Recepie> Components {get; set;}

        public override string ToString(){
            return $"{Name}";
        }

        public bool IsTimesUsed(string comp){
            if(comp == "ORE")
                return true;
            var full = Program.dic[Name];

            if(full.Components
                .Any(c => c.Name == comp)){
                Console.WriteLine("{0} is used in {1}", comp, Name);
                return true;
                
            }
            return full.Components
                .Select(c => Program.dic[c.Name])
                .Any(c => c.IsTimesUsed(comp));
        }

        public void Dump(){
            Console.WriteLine("Name: {0}; Components: {1}", Name,
                string.Join(", ", Components.Select(c => $"{c.Amount} {c.Name}" )));
            List<Recepie> list = Components
                .Where(c => c.Name != "ORE")
                .Select(c => Program.dic[c.Name]).ToList();
            list.ForEach(i => i.Dump());
        }

        internal int Cost()
        {
            while(Components.Count > 1){
                foreach (var comp in Components){
                    if(Components.Any(c => c.IsTimesUsed(comp.Name))){
                        Console.WriteLine("delaying {0}", comp.Name);
                        continue;
                    }

                    Resolve(this, comp);

                    break;
                }
            }
            return Components.First().Amount;
        }

        private void Resolve(Recepie recepie, Recepie comp)
        {
            recepie.Components.Remove(comp);
            var fullComp = Program.dic[comp.Name];
            var compCost = fullComp.Amount;
            var totalAmount = (int)Math.Ceiling((decimal)comp.Amount / compCost);
            foreach (var subItem in fullComp.Components)
            {
                var subAmount = subItem.Amount * totalAmount;
                var newComp = recepie.Components.Where(c => c.Name == subItem.Name).FirstOrDefault();
                if(newComp == null) {
                    newComp = new Recepie(){
                        Amount = subAmount,
                        Name = subItem.Name
                    };
                    recepie.Components.Add(newComp);
                } else {
                    newComp.Amount += subAmount;
                }
            }
            Console.WriteLine("Resolved: {0}; Components: {1}", comp.Name,
                string.Join(", ", recepie.Components.Select(c => $"{c.Amount} {c.Name}" ))
            );
        }

        public override bool Equals(object obj)
        {
            return obj is Recepie recepie &&
                   Name == recepie.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }

    public static class Extensions
    {
        public static IEnumerable<string> Splizz(this string str, params string[] seps){
            if(seps == null || seps.Length == 0)
                seps = new[]{";", ","};
            return str.Split(seps,StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
        }
        public static IEnumerable<IEnumerable<string>> Splizz(this IEnumerable<string> enumerable, params string[] seps){
            foreach (var item in enumerable)
            {
                yield return item.Splizz(seps);
            }
        }
         public static IEnumerable<IEnumerable<string>> Splizz(this string[] enumerable, params string[] seps){
            foreach (var item in enumerable)
            {
                yield return item.Splizz(seps);
            }
        }

        public static IEnumerable<T> Many<T>(this IEnumerable<IEnumerable<T>> enumerable){
            foreach (var item in enumerable)
            {
                foreach (var nestedItem in item)
                {
                     yield return nestedItem;
                }
            }
        }

        public static string ToCommaString<T>(this IEnumerable<T> enumerable, string seperator = ", "){
            return string.Join(seperator, enumerable.Select(e => e.ToString()));
        }

        public static int ManhattenDistance(this Point point, Point other){
            return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
        } 

        public static IEnumerable<T> RegExParse<T>(this IEnumerable<string> enumerable, string pattern)
            where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).ToArray();
            var regex = new Regex(pattern, RegexOptions.Compiled);
            foreach (var item in enumerable)
            {
                Match match = regex.Match(item);
                var t = new T();
                foreach (var group in match.Groups.Keys.Where(k => !int.TryParse(k, out var _)))
                {
                    var capture = match.Groups[group];
                    var prop = props.FirstOrDefault(p => string.Equals(p.Name, group, StringComparison.OrdinalIgnoreCase));
                    if(prop == null)
                        throw new Exception($"Property '{group}' not found on type '{type.Name}', candidates were {props.ToCommaString()}");

                    prop.SetValue(t, Convert.ChangeType(capture.Value, prop.PropertyType));  
                }

                yield return t;
            }
        }
    }
}
