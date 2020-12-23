using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Foo<TPoint> : IHasPosition<TPoint>
        where TPoint : IPointish
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string A { get; set; }
        public string B { get; set; }

        public TPoint Pos { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var cups = LoadCups("input.txt");

            var circle = MakeMoves(cups, 100);
            SelectAllAfterCupLabeledOne(circle).ToCommaString("").AsResult1();

            var cupsPart2 = cups.Concat(Enumerable.Range(cups.Max() + 1, 1_000_000 - cups.Count)).ToList();
            var circlePart2 = MakeMoves(cupsPart2, 10_000_000);
            SelectAllAfterCupLabeledOne(circlePart2).Take(2).MultiplyAll().AsResult2();

            Report.End();
        }

        private static List<int> SelectAllAfterCupLabeledOne(List<int> circle)
        {
            var oneIndex = circle.IndexOf(1);
            var res = new List<int>(circle.Count);
            for (int i = 0; i < circle.Count - 1; i++)
            {
                oneIndex++;
                if (oneIndex >= circle.Count)
                {
                    oneIndex = 0;
                }
                res.Add(circle[oneIndex]);
            }

            return res;
        }

        private static List<int> MakeMoves(List<int> foos, int moves)
        {
            var circle = new LinkedList<int>(foos);
            var lookup = BuildLookupDictionary(foos, circle);

            var currentCup = circle.First;
            var max = foos.Max().Debug("Max");
            var hand = new Stack<LinkedListNode<int>>(3);
            for (int i = 0; i < moves; i++)
            {
                Pick(3, currentCup, hand);
                var destinationCup = CalcDestinationCup(lookup, currentCup, max, hand);

                PutBack(hand, destinationCup, lookup);
                currentCup = SelectNextCurrentCup(currentCup);
            }

            return circle.ToList();
        }

        private static LinkedListNode<int> SelectNextCurrentCup(LinkedListNode<int> currentCup)
        {
            var circle = currentCup.List;
            currentCup = currentCup.Next;
            if (currentCup == null)
            {
                currentCup = circle.First;
            }

            return currentCup;
        }

        private static void PutBack(Stack<LinkedListNode<int>> hand, LinkedListNode<int> destinationCup, Dictionary<int, LinkedListNode<int>> lookup)
        {
            while (hand.Count > 0)
            {
                var label = hand.Pop().Value;
                lookup[label] = destinationCup.List.AddAfter(destinationCup, label);
            }
        }

        private static LinkedListNode<int> CalcDestinationCup(Dictionary<int, LinkedListNode<int>> lookup, LinkedListNode<int> currentCup, int max, Stack<LinkedListNode<int>> hand)
        {
            int destinationCupValue = Enumerable.Range(1, 4).Select(i => currentCup.Value - i).Except(hand.Select(h => h.Value)).Max();
            if (destinationCupValue <= 0)
            {
                destinationCupValue = Enumerable.Range(1, 4).Select(i => max + 1 - i).Except(hand.Select(h => h.Value)).Max();
            }
            var destinationCup = lookup[destinationCupValue];
            return destinationCup;
        }

        private static void Pick(int amount, LinkedListNode<int> currentCup, Stack<LinkedListNode<int>> hand)
        {
            var picky = currentCup;
            for (int j = 1; j <= amount; j++)
            {
                picky = picky.Next;
                if (picky == null)
                {
                    picky = currentCup.List.First;
                }
                hand.Push(picky);
            }

            foreach (var pick in hand)
            {
                pick.List.Remove(pick);
            }
        }

        private static Dictionary<int, LinkedListNode<int>> BuildLookupDictionary(List<int> foos, LinkedList<int> circle)
        {
            var lookup = new Dictionary<int, LinkedListNode<int>>(foos.Count);
            var cup = circle.First;
            while (cup != null)
            {
                lookup[cup.Value] = cup;
                cup = cup.Next;
            }

            return lookup;
        }

        public static List<int> LoadCups(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .SelectMany(s => s.Select(c => c.ToString()))
                .Select(int.Parse)
                .ToList();
        }
    }
}
