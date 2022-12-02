using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Round
    {
        public string A { get; set; }
        public string B { get; set; }
    }

    class Program
    {
        enum Shape{
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        enum Outcome{
            Loss = 0,
            Draw = 3,
            Win = 6,
        }
        static readonly Dictionary<string, Shape> opponentsMoves = new(){
                {"A", Shape.Rock},
                {"B", Shape.Paper},
                {"C", Shape.Scissors},
            };
             static readonly Dictionary<string, Shape> myMoves = new(){
                {"X", Shape.Rock},
                {"Y", Shape.Paper},
                {"Z", Shape.Scissors},
            };
             static readonly Dictionary<string, Outcome> desiredOutcomes = new(){
                {"X", Outcome.Loss},
                {"Y", Outcome.Draw},
                {"Z", Outcome.Win},
            };
        static void Main(string[] args)
        {
            Report.Start();
            var rounds = LoadRounds("input.txt");

            rounds.Select(round => CalcPart1(round)).Sum().AsResult1();
            rounds.Select(round => CalcPart2(round)).Sum().AsResult2();


            Report.End();
        }

        private static long CalcPart1(Round round)
        {
            var opponentsMove = opponentsMoves[round.A];
            var myMove = myMoves[round.B];

            return CalcMyScore(opponentsMove, myMove);
        }

        private static long CalcPart2(Round round)
        {
            var opponentsMove = opponentsMoves[round.A];
            var desiredOutcome = desiredOutcomes[round.B];

            Shape myMove = (desiredOutcome, opponentsMove) switch {
                (Outcome.Draw, _) => opponentsMove,
                (Outcome.Win, Shape.Scissors) => Shape.Rock,
                (Outcome.Win, Shape.Rock) => Shape.Paper,
                (Outcome.Win, Shape.Paper) => Shape.Scissors,
                (Outcome.Loss, Shape.Scissors) => Shape.Paper,
                (Outcome.Loss, Shape.Rock) => Shape.Scissors,
                (Outcome.Loss, Shape.Paper) => Shape.Rock,
            };

            return CalcMyScore(opponentsMove, myMove);
        }

        private static long CalcMyScore(Shape opponentsMove, Shape myMove)
        {
            Outcome outcome = (opponentsMove, myMove) switch {
                (Shape.Scissors, Shape.Paper) => Outcome.Loss,
                (Shape.Scissors, Shape.Rock) => Outcome.Win,
                (Shape.Paper, Shape.Scissors) => Outcome.Win,
                (Shape.Paper, Shape.Rock) => Outcome.Loss,
                (Shape.Rock, Shape.Paper) => Outcome.Win,
                (Shape.Rock, Shape.Scissors) => Outcome.Loss,
                (_, _) => Outcome.Draw,
            };
            
            return (long)outcome + (long)myMove;
        }

        public static List<Round> LoadRounds(string inputTxt)
        {
            return File
               .ReadAllLines(inputTxt)
               .Where(s => !string.IsNullOrWhiteSpace(s))
               .Select(s => s.Trim())
               .Select(s => s.ParseRegex(@"^([^ ]+) ([^ ]+)$", m => new Round()
               {
                   A = (m.Groups[1].Value),
                   B = (m.Groups[2].Value),
               }))
               .ToList();
        }
    }
}
