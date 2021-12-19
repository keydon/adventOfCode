using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Explosion
    {
        public Explosion(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public int? Left {get; set;}
        public int? Right {get; set;}

        public bool IsFirst() {
            return Left != null && Right != null;
        }
    }
    record SnailfishNumber 
    {
        public SnailfishNumber(){}
        public SnailfishNumber(int number){
            IsRegularNumber = true;
            RegularNumber = number;
        }
        public SnailfishNumber Left { get; set; }
        public SnailfishNumber Right { get; set; }

        public int RegularNumber { get; set; }
        public bool IsRegularNumber { get; set; }

        public override int GetHashCode()
        {
            if (IsRegularNumber)
                return RegularNumber.GetHashCode();
            return HashCode.Combine(Left, Right);
        }

        public override string ToString()
        {
            if (IsRegularNumber)
                return RegularNumber.ToString();
            return $"[{Left},{Right}]";
        }

        internal SnailfishNumber Add(SnailfishNumber other)
        {
            var sum = new SnailfishNumber(){
                Left = this.DeepCopy(),
                Right = other.DeepCopy()
            };
            while(sum.Reduce()){ }
            return sum;
        }
        
        public SnailfishNumber DeepCopy()
        {
            if (IsRegularNumber)
                return new SnailfishNumber(this);

            return new SnailfishNumber(this)
            {
                Left = Left.DeepCopy(),
                Right = Right.DeepCopy()
            };
        }

        private bool Reduce()
        {
            var nestedPair = Explode();
            if(nestedPair != null) {
                return true;
            }
            return Split();
        }

        private bool Split()
        {
            if (IsRegularNumber){
                if (RegularNumber >= 10){
                    var splitN = RegularNumber / 2D;
                    IsRegularNumber = false;
                    Left = new SnailfishNumber((int)Math.Floor(splitN));
                    Right = new SnailfishNumber((int)Math.Ceiling(splitN));
                    return true;
                }
                return false;
            }
            if (Left.Split())
                return true;

            return Right.Split();
        }

        private Explosion Explode(int level = 0)
        {
            if (!Left.IsRegularNumber) {
                var ex = Left.Explode(level + 1);
                if (ex != null) {
                    if (ex.IsFirst()) {
                        Left = new SnailfishNumber(){
                            RegularNumber = 0,
                            IsRegularNumber = true
                        };
                        Right.AddLeft(ex.Right.Value);
                        ex.Right = null;
                        return ex;
                    } else if (ex.Left != null){
                        return ex;
                    } else if(ex.Right != null) {
                        Right.AddLeft(ex.Right.Value);
                        ex.Right = null;
                        return ex;
                    } else {
                        return ex;
                    }
                }
            }
            if (!Right.IsRegularNumber) {
                var ex = Right.Explode(level + 1);
                if (ex != null) {
                    if (ex.IsFirst()) {
                        Right = new SnailfishNumber(){
                            RegularNumber = 0,
                            IsRegularNumber = true
                        };
                        Left.AddRight(ex.Left.Value);
                        ex.Left = null;
                        return ex;
                    } else if (ex.Right != null){
                        return ex;
                    } else if (ex.Left != null){
                        Left.AddRight(ex.Left.Value);
                        ex.Left = null;
                        return ex;
                    } else {
                        return ex;
                    }
                }
            }
            if(level >= 4)
                return new Explosion(
                    Left.RegularNumber,
                    Right.RegularNumber
                );
            return null;
        }

        private void AddRight(int number)
        {
            if(IsRegularNumber) {
                RegularNumber += number;
            } else {
                Right.AddRight(number);
            }
        }

        private void AddLeft(int number)
        {
            if(IsRegularNumber) {
                RegularNumber += number;
            } else {
                Left.AddLeft(number);
            }
        }

        internal long CalcMagnitude()
        {
            if(IsRegularNumber)
                return RegularNumber;
            return 3L * Left.CalcMagnitude() + 2L * Right.CalcMagnitude();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            //foos = LoadFoos("sample3.txt");
            var numbers = new LinkedList<SnailfishNumber>();
            foreach (var line in foos)
            {
                var n = ParseSnailfishNumber(line);
                numbers.AddLast(n);
            }

            var sum = numbers.Aggregate((a, b) =>
            {
                return a.Add(b);
            });

            sum.Debug("SUM FIN");
            sum.CalcMagnitude().AsResult1();

            var numbers2 = new LinkedList<SnailfishNumber>();
            foreach (var line in foos)
            {
                var n = ParseSnailfishNumber(line.Debug("LINE"));
                numbers2.AddLast(n);
            }


            var mags = numbers2.SelectMany(a => numbers2, (a,b) => {
                if(a == b){
                    return (0, null, null, null);
                }
                a.Debug("a");
                b.Debug("b");
                var sumAB = a.Add(b);
                sumAB.Debug("suuuuuuum");
                var magAB = sumAB.CalcMagnitude();
                return (magAB, sumAB, a ,b);
            }).ToList();
            
            var biggest = mags.OrderByDescending(r => r.Item1).First();
            biggest.Debug("biggest");
            biggest.magAB.AsResult2();

            Report.End();
        }


        private static SnailfishNumber ParseSnailfishNumber(string line)
        {
            var ll = new LinkedList<char>(line.Select(c => c));
            return ParseSnailfishNumber(ll.First);
        }

        private static SnailfishNumber ParseSnailfishNumber(LinkedListNode<char> first)
        {
            var list = first.List;
            var prev = first.Previous;
            var left = new SnailfishNumber();
            if ( first.Value == '['){
                left = ParseSnailfishNumber(first.Next);
                first = prev?.Next;
                if(first == null)
                    return left;
                
            } else {
                left.RegularNumber = int.Parse(first.Value.ToString());
                left.IsRegularNumber = true;
            }
            var comma = first.Next;
            if(comma.Value == ']') {
                list.Remove(first);
                return left;   
            }
            var second = comma.Next;
            var right = new SnailfishNumber();
            if ( second.Value == '['){
                right = ParseSnailfishNumber(second);
            } else {
                right.RegularNumber = int.Parse(second.Value.ToString());
                right.IsRegularNumber = true;
                list.Remove(second);
            }
            list.Remove(comma.Next);
            list.Remove(comma);
            list.Remove(first);

            return new SnailfishNumber(){
                Left = left,
                Right = right
            };
        }

        public static List<string> LoadFoos(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())

             //.GroupByLineSeperator()
             //.Parse2DMap((p, t) => new Foo<Point2> { Pos = p, A = t })
             //.SelectMany(r => r.Splizz(",", ";"))
             //.Where(a => a.foo == '#')
             //.Select(int.Parse)
             //.Select(long.Parse)  
             //  .Select(s => s.ParseRegex(@"^mem\[(\d+)\] = (\d+)$", m => new Foo<Point2>()
             //  {
             //      X = int.Parse(m.Groups[1].Value),
             //      Y = int.Parse(m.Groups[2].Value),
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
