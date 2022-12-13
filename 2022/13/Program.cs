using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Pair
    {
        public List<Pair> Subs { get; set; }
        public Pair Left { get; set; }
        public Pair Right { get; set; }
        public int Index { get; set; }

        public string Typo { get; set; }
        public int Value { get; set; }

        public Pair Parent {get; set; }

        public static Pair Max => Pair.ParseInt("99");
        public static Pair Min => Pair.ParseInt("0");
        public static Pair Neutral => Pair.ParseInt("50");

        public Pair Wrap(){
            return new Pair(){
                Typo = "list",
                Subs = new List<Pair>() { this }
            };
        }
        

        internal static Pair Parse(string listOrValue, int index)
        {
            var ll = new LinkedList<string>(listOrValue.SelectStrings());
            var first = ll.First;
            return Pair.Parse(ref first).Subs.First();
        }
        
        internal static Pair Parse(ref LinkedListNode<string> node){
            var pair = new Pair(){
                Subs = new List<Pair>(),
            };
            var list = node.List;
            while(true) {
                if (node == null){
                    node.Debug("NUUU");
                    return pair ;
                }
                node.Value.Debug("IN");
                if (node.Value == "]"){
                    pair.Typo = "list";
                    if(pair.Subs.Count == 0){
                    pair.Typo = "empty";
                    }
                    node = node.Next;
                    return pair;
                }
                if (node.Value == "[") {
                    node = node.Next;
                    var sub = Pair.Parse(ref node);
                    pair.Subs.Add(sub);
                    continue;
                }
                if (node.Value == ","){
                    node = node.Next;
                    //var sub = Pair.Parse(ref node);
                    //pair.Subs.Add(sub);
                    continue;
                }
                if (int.TryParse(node.Value, out int val1)){
                    node = node.Next;
                    if (int.TryParse(node.Value, out int val2)){
                        node = node.Next;
                        pair.Subs.Add(Pair.ParseInt(val1.ToString() + val2.ToString()));
                        continue;
                    } else {
                        pair.Subs.Add(Pair.ParseInt(val1.ToString()));
                        continue;
                    }
                }
            }
        }

        internal static Pair ParseInt(string value)
        {
            if (int.TryParse(value, out int val)){
                return new Pair(){
                    Typo = "int",
                    Value = val,
                };
            }
            throw new Exception("not an int");
        }

     

        public override string ToString()
        {
            if(Typo == "root"){
               return Left.ToString() + " \r\n vs.  " + Right.ToString();
            }
            if(Typo == "int")
                return Value.ToString();
            return "[" + Subs.ToCommaString() + "]";
        }

        public static bool isLessThan(Pair l, Pair r)
        { 
            return (l.Typo, r.Typo) switch {
                ("list", "list") => ((l.Subs.Count == r.Subs.Count) switch{
                    true => l.Subs.Zip(r.Subs),
                    false => l.Subs.Count > r.Subs.Count 
                        ? l.Subs.Zip(r.Subs.Concat(new List<Pair>(){Pair.Min}))
                        : l.Subs.Take(r.Subs.Count).Zip(r.Subs)
                        
                }).Select((p) => Pair.isLessThan(p.First, p.Second))
                    .SkipWhile(a => !a) .All(a => a),
                ("list", "int") => Pair.isLessThan(l, r.Wrap()),
                ("int", "list") => Pair.isLessThan(l.Wrap(), r),
                ("empty", "empty") => true,
                ("empty", _) => throw new Exception("true"),
                (_, "empty") => throw new Exception("false"),
                ("int", "int") => 
                    l.Value.Debug("LEFT") > r.Value.Debug("RIGHT")
                        ? throw new Exception("false")
                        : l.Value == r.Value
                         ? true
                         : throw new Exception("true")
            };
            
        }
    }
    class Program
    {
        static void Main(string[] args)
        {


            Report.Start();
            //var foos = LoadFoos("input.txt");
            var foos = LoadFoos("sample4.txt");
            var ok = new List<Pair>();
            var res = new List<bool>();

/*
            var mine = File.ReadAllLines("debug.txt").Select(s => bool.Parse(s));
            var his = File.ReadAllLines("debug-markus.txt").Select(s => bool.Parse(s));
            var x = mine.Zip(his)
                .Select((p, i) => (p, i))
                .Where(p => p.p.First != p.p.Second)
                .Select(p => foos[p.i])
                .Debug()
                .ToList();
                throw new Exception("ydfasd");
*/
            foreach (var item in foos)
            {
                var isLess = true;
                try {
                    isLess = Pair.isLessThan(item.Left, item.Right);
                }
                catch(Exception e){
                    isLess = bool.Parse(e.Message);
                }
                res.Add(isLess);
                if(!isLess)
                    item.Debug(isLess.ToString());
                if(isLess) ok.Add(item);
                
            }

            ok.Select(item => item.Index+1L).Sum().AsResult1();
            //File.WriteAllLines("debug.txt", res.Select(r => r.ToString()));

            


            Report.End();
        }

        public static List<Pair> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
             .GroupByLineSeperator()
             .Select((g, i) => (group: i, left: g.First(), right: g.Last()))
             .Select(g => new Pair(){Index = g.group, Left = Pair.Parse(g.left, g.group*3), Right= Pair.Parse(g.right, g.group*3+1), Typo = "root"})
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
             //      A = m.Groups[1].Value,
             //      B = m.Groups[2].Value,
             //  }))
             //.Where(f = f)
             //.ToDictionary(
             //    (a) => new Vector3(a.x, a.y),
             //    (a) => new Foo(new Vector3(a.x, a.y))
             //);
             //.ToArray()
             .ToList()
            ;

            return foos;
        }
    }
}
