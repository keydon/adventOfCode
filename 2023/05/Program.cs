using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record State
    {
        public string Unit { get; set; }
        public List<LongRange> Values { get; set; }

    }


    record Conversion{
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public List<ConvRange> Ranges { get; set; }

        internal State Apply(State currentState)
        {
            var newState = new State(){
                Unit = ToUnit,
                Values = currentState.Values.SelectMany(
                    v => {
                        var conversions = Ranges.Where(r => r.Matches(v)).Select(r => r.Convert(v)).ToList();
                        if(conversions.Count == 0)
                            return new List<LongRange>(){ v};
                        return conversions;
                    }
                ).ToList()
            };
            return newState;
        }
    }

    record ConvRange {
        public long SourceStart { get; set; }
        public long DestinationStart { get; set; }
        public long Offset {get; set; }
        public long Length { get; set; }
        public LongRange Range {get; set; }

        internal virtual LongRange Convert(LongRange v)
        {
            var intersect = v.Intersect(Range);
            var offset = DestinationStart - SourceStart; 
            var converted = new LongRange(){
                Start = intersect.Start + offset,
                End = intersect.End + offset
            };
            converted.Debug("Offeset: " + Offset);
            return converted;
        }

        internal virtual bool Matches(LongRange v)
        {
            return v.DoesIntersect(Range);
        }
    }

    record NoopConvRange : ConvRange {
        internal override LongRange Convert(LongRange v)
        {
            return v;
        }

        internal override bool Matches(LongRange v)
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
            currentState.Values.Count().Debug("Exp");
            while(true){
                if(targetUnit == currentState.Unit){
                    break;
                }
                var conv = convMap[currentState.Unit];
                currentState = conv.Apply(currentState);
                currentState.Debug(currentState.Unit);
                currentState.Values.Peek().ToList();
            }

            currentState.Debug("Final");
            currentState.Values.Select(r => r.Start).Min().AsResult2();


            Report.End();
        }

        private static IEnumerable<long> Expand(List<long> values)
        {
            for (int i = 0; i < values.Count; i+=2)
            {
                var start =  values[i];
                var iters = values[i+1];
                for (long l = start; l < start + iters; l++)
                {
                    yield return l;
                }
            }
        }

        public static INput LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
               // .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             .GroupByLineSeperator().ToList();

             var init = foos.First().First().Splizz(":", " ").ToList();

            List<LongRange> longRanges = init.Skip(1)
                .Select(long.Parse)
                .Chunk(2)
                .Select(chunk => new LongRange(){
                     Start = chunk.First(),
                     End = chunk.First() + chunk.Last() -1
                }).ToList();

             var initState = new State() {
                 Unit = init.First(),
                 Values = longRanges
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
                          Offset = parts.First() -parts[1],
                          Length = parts.Last(),
                          Range = new LongRange(){
                             Start = parts[1],
                             End = parts[1] + parts.Last() -1
                          }
                    };
                }).ToList();
                //convRanges.Add(new NoopConvRange());
                
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
