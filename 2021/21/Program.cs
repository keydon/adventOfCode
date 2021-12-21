using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Dice {
        private readonly int sides;
        private readonly int start;
        private readonly IEnumerator<int> enumerator;
        public long RollCount {get; set;} = 0;
        public string DebugPrefix = "Dice";

        public Dice(int sides, int start = 1){
            this.sides = sides;
            this.start = start;
            var rolls = PrepareRolling();
            enumerator = rolls.GetEnumerator();
        }

        public int Roll(){
            RollCount++;
            enumerator.MoveNext();
            return enumerator.Current;//.Debug(DebugPrefix + "Rolled");
        }

        private IEnumerable<int> PrepareRolling(){
            var s = start;
            while(true){
                var rolls = Enumerable.Range(s, sides-s+1);
                foreach (var roll in rolls)
                {
                    yield return roll;
                }
                s = 1;
            }
        }
    }



    
    record Foo
    {
        public int Id { get; set; }
        public int Posi { get; set; }
        public long Score { get; set; }

        public Dice Pawn { get; set;}

        public string A { get; set; }
        public string B { get; set; }

        public void MakeTurn(Dice dice){
            //Posi.Debug(Id +"Starts at");
            var sum = Enumerable.Range(0, 3).Select(turn => dice.Roll()).Sum().Debug(Id+ " Moves");
            var newPos = Enumerable.Range(0, sum).Select(movie => Pawn.Roll()).ToList().Last().Debug(Id + " EndPos");
            Score += newPos;
            Score.Debug(Id + " Score");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foos = LoadFoos("input.txt");
            foos = LoadFoos("sample.txt");
            var player1 = foos[0];
            var player2 = foos[1];

            partTwo(player1.Posi, player2.Posi, 21);
            return;

            player1.Pawn = new Dice(10, player1.Posi);
            player1.Pawn.DebugPrefix = "p1";
            player1.Pawn.Roll();
            player2.Pawn = new Dice(10, player2.Posi);
            player2.Pawn.DebugPrefix = "p2";
            player2.Pawn.Roll();

            var dice = new Dice(100);
            while(true){
                player1.MakeTurn(dice);
                if(player1.Score >=1000){
                    player1.Id.Debug("WINS");
                    player1.Score.Debug("Score");
                    dice.RollCount.Debug("RollCuunt");
                    (player2.Score * dice.RollCount).AsResult1();
                    break;
                }                

                player2.MakeTurn(dice);
                if(player2.Score >=1000){
                    player2.Id.Debug("WINS");
                    player2.Score.Debug("Score");
                    dice.RollCount.Debug("RollCuunt");
                    (player1.Score * dice.RollCount).AsResult1();
                    break;
                }
            }


            Report.End();
        }

        private static void partTwo(int posi1, int posi2, int targetPoints)
        {
            var p1wins = 0L;
            var p2wins = 0L;
            var qDice = quantumDice().Peek("dice").ToList();
            qDice.Count.Debug("dc");
            Console.ReadKey();
            var state = new Dictionary<(int p1pos, int p2pos),Dictionary<(int p1points, int p2points), long>>();
            var init = new Dictionary<(int p1points, int p2points), long>
            {
                {(0, 0), 1L}
            };
            state.Add((posi1, posi2), init);
            while (true) {
                var keys = state.Keys.ToList();
                if (keys.Count == 0) {
                    keys.Count.Debug("k");
                    p1wins.Debug("p1wins");
                    p2wins.Debug("p2wins");
                    // 444356092776315
                    // 11997614504960505
                    // 20780247974

                    break;
                }
                var lowestPositions = getLowestPosition(keys).ToList();
                foreach (var lowPosition in lowestPositions)
                {
                    lowPosition.Debug("lowpos");
                    var scores = state[lowPosition];
                    state.Remove(lowPosition);

                    var (pos1, pos2) = lowPosition;

                    foreach (var d in qDice)
                    {
                        var p1pos = pos1 + d.augen1;
                        var p2pos = pos2 + d.augen2;
                        if(p1pos > 10)
                            p1pos %= 10;
                        if(p2pos > 10)
                            p2pos %= 10;
                        var newPos = (p1pos, p2pos);
                        //newPos.Debug("npos " + (d.augen1, d.augen2));
                        
                        var val = state.TryGetDefAdd(newPos, x => new Dictionary<(int p1points, int p2points), long>());
                        foreach (var score in scores.Where(s => s.Value > 0))
                        {
                            var p1Points = score.Key.p1points + p1pos;
                            var p2Points = score.Key.p2points + p2pos;
                            
                            var vv = score.Value * d.occurences;
                                var points = (p1Points, p2Points);
                            if(p1Points >= targetPoints){
                                p1wins += vv;
                                //Console.WriteLine($"{lowPosition} -> {newPos}: {score.Value}=**{d}**>>{vv} ::  {score.Key}-->{points} == {p1wins}");
                                //Console.ReadKey();
                            } else if (p2Points >= targetPoints){
                                p2wins += vv;
                            } else {
                                val.Change(points, v => v + vv);
                                //Console.WriteLine($"{lowPosition} -> {newPos}: {score.Value}=**{d}**>>{vv} ::  {val[points]}");
                                //Console.ReadKey();
                            }
                        }
                        if (val.Count == 0){
                            state.Remove(newPos);
                        } else {
                        }
                    }
                }
            }
        }

        
        private static IEnumerable<(int augen1, int augen2, int occurences)> quantumDice()
        {
            var dices = new Dictionary<(int,int),int>();
            for (int d1 = 1; d1 < 4; d1++)
            {
                for (int d2 = 1; d2 < 4; d2++)
                {
                    for (int d3 = 1; d3 < 4; d3++)
                    {
                        var key1 = d1 + d2 + d3;

                        for (int dd1 = 1; dd1 < 4; dd1++)
                        {
                            for (int dd2 = 1; dd2 < 4; dd2++)
                            {
                                for (int dd3 = 1; dd3 < 4; dd3++)
                                {
                                    var key2 = dd1 + dd2 + dd3;
                                    dices.Change((key1,key2), a => a + 1);
                                }   
                            }   
                        }
                    }   
                }   
            }
            return dices.Select(kvp => (augen1: kvp.Key.Item1, augen2: kvp.Key.Item2, occurences: kvp.Value));
        }

        private static IEnumerable<(int p1pos, int p2pos)> getLowestPosition(List<(int p1pos, int p2pos)> keys)
        {
            var min1 = keys.Min(k => k.p1pos);
            var min2 = keys.Min(k => k.p2pos);

            if(min1 <= min2) {
                return keys.Where(k => k.p1pos == min1);
            }
            return keys.Where(k => k.p2pos == min2);
        }

        public static List<Foo> LoadFoos(string inputTxt)
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
              .Select(s => s.ParseRegex(@"^Player (\d+) starting position: (\d+)$", m => new Foo()
               {
                   Id = int.Parse(m.Groups[1].Value),
                   Posi = int.Parse(m.Groups[2].Value),
               }))
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
