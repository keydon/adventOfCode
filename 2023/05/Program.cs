using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record State
    {
        public string Unit { get; set; }
        public List<long> Values { get; set; }

    }

    record Conversion{
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public List<ConvRange> Ranges { get; set; }

        internal State Apply(State currentState)
        {
            var newState = new State(){
                Unit = ToUnit,
                Values = currentState.Values.Select(
                    v => Ranges.Where(r => r.Matches(v)).Select(r => r.Convert(v)).First()
                ).ToList()
            };
            return newState;
        }
    }

    record ConvRange {
        public long SourceStart { get; set; }
        public long DestinationStart { get; set; }
        public long Length { get; set; }

        internal virtual long Convert(long v)
        {
            var offset = v - SourceStart;
            return DestinationStart + offset;
        }

        internal virtual bool Matches(long v)
        {
            return v >= SourceStart 
            && v <= SourceStart + Length;
        }
    }

    record NoopConvRange : ConvRange {
        internal override long Convert(long v)
        {
            return v;
        }

        internal override bool Matches(long v)
        {
            return true;
        }
    }

    record INput {
        public State State {get; set;}
        
        public List<Conversion> Convs { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample.txt");

            
            var convMap = foos.Convs.ToDictionary(k => k.FromUnit, v => v);

            var targetUnit = "location";
            var currentState = foos.State;
            while(true){
                if(targetUnit == currentState.Unit){
                    break;
                }
                var conv = convMap[currentState.Unit];
                currentState = conv.Apply(currentState);
            }

            currentState.Debug("Final");
            currentState.Values.Min().AsResult1();


            Report.End();
        }

        public static INput LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
               // .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             .GroupByLineSeperator().ToList();

             var init = foos.First().First().Splizz(":", " ").ToList();
             var initState = new State() {
                 Unit = init.First(),
                 Values = init.Skip(1).Select(long.Parse).ToList()
             };

             var convs = foos.Skip(1)
             .Select(g => {
                var header = g.First().Splizz("-to-", " ").ToList();

                 List<ConvRange> convRanges = g.Skip(1).Select(l => {
                    var parts = l.Splizz(" ")
                        .Select(long.Parse)
                        .ToList();
                    return new ConvRange(){
                         DestinationStart = parts.First(),
                          SourceStart = parts[1],
                           Length = parts.Last()
                    };
                }).ToList();
                convRanges.Add(new NoopConvRange());
                
                var conv = new Conversion(){
                 FromUnit = header.First(),
                 ToUnit = header[1],
                 Ranges = convRanges
                };
                
                return conv;
                }).ToList();


return new INput(){
     Convs = convs,
      State = initState
};

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
        }
    }
}
