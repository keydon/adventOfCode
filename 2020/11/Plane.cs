using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace aoc
{
    public class Plane<T>
    {
        public readonly Dictionary<Point, T> Dic = new Dictionary<Point, T>();
        public readonly LinkedList<T> AllSpaces = new LinkedList<T>();
        private readonly Func<T, Point> pointGetter;
        public Plane(Func<T, Point> PointGetter)
        {
            pointGetter = PointGetter;
        }

        public void Add(T item)
        {
            var p = pointGetter(item);
            Dic[p] = item;
            AllSpaces.AddLast(item);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
            Console.WriteLine($"total: {AllSpaces.Count}");
        }

        public IEnumerable<T> GetNeighbours(T item)
        {
            return GetNeighbours(pointGetter(item));
        }

        public IEnumerable<T> GetNeighbours(Point p)
        {
            return p.GetNeighbours()
                .Where(Dic.ContainsKey)
                .Select(p => Dic[p]);
        }

        public T GetDef(Point coordinates)
        {
            if (Dic.TryGetValue(coordinates, out var target))
                return target;
            return default;
        }
    }
}
