using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var orbitMap = File
                .ReadAllLines("input.txt")
                .Select(l => {
                    var orbs = l.Splizz(")").ToArray();
                    return new {Inner = orbs[0], Outter = orbs[1]};
                })
                .Aggregate(new OrbitMap(), (map, orb) => map.AddOrbit(orb.Inner, orb.Outter));
            
            stopwatch.Stop();
            Console.WriteLine("Orbitmap checksum: {0}", orbitMap.GetCecksum());
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            var you = orbitMap.GetPlanet("YOU");
            var san = orbitMap.GetPlanet("SAN");

            var yourPlants = you.GetInnerPlanets().ToList();
            var sansPlanets = san.GetInnerPlanets().ToList();
            var intersect = yourPlants.Intersect(sansPlanets);
            var union = yourPlants.Union(sansPlanets);

            var transfers = union.Except(intersect).Count();

            stopwatch.Stop();
            Console.WriteLine("Amount of transfers from YOU to SAN: {0}", transfers);
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    internal class Planet
    {
        public Planet(string name, Planet isOrbitingAround = null)
        {
            Name = name;
            IsOrbitingAround = isOrbitingAround;
        }
        public string Name { get; set; }
        public Planet IsOrbitingAround { get; set; }

        public IEnumerable<Planet> GetInnerPlanets()
        {
            if (IsOrbitingAround == null)
                yield break;

            yield return IsOrbitingAround;
            foreach (var inner in IsOrbitingAround.GetInnerPlanets())
            {
                yield return inner;
            }
        }

        public int OrbitCount => IsOrbitingAround?.OrbitCount + 1 ?? 0;
        
        protected bool Equals(Planet other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Planet) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }

    internal class OrbitMap
    {
        private readonly Dictionary<string, Planet> planetsDict = new Dictionary<string, Planet>();

        public OrbitMap()
        {
        }

        internal OrbitMap AddOrbit(string inner, string outter)
        {
            if(!planetsDict.TryGetValue(inner, out var innerPlanet))
            {
                innerPlanet = new Planet(inner);
                planetsDict.Add(inner, innerPlanet);
            }

            if(!planetsDict.TryGetValue(outter, out var outerPlanet))
            {
                outerPlanet = new Planet(outter, innerPlanet);
                planetsDict.Add(outter, outerPlanet);
            }
            else
            {
                if(outerPlanet.IsOrbitingAround != null)
                    throw new Exception("outter already exists");
                outerPlanet.IsOrbitingAround = innerPlanet;
            }

            return this;
        }

        public Planet GetPlanet(string name)
        {
            return planetsDict[name];
        }

        public int GetCecksum()
        {
            return planetsDict
                .Select(p => p.Value.OrbitCount)
                .Sum();
        }
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
}


