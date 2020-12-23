using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var cardDecks = LoadCardDecks("input.txt")
                .Select(deck => deck.Skip(1).Select(card => int.Parse(card)))
                .ToList();

            var winnerPart1 = PlayGamePart1(cardDecks[0].ToLinkedList(), cardDecks[1].ToLinkedList());
            CalculateScore(winnerPart1).AsResult1();


            var winnerPart2 = PlayGamePart2(cardDecks[0].ToLinkedList(), cardDecks[1].ToLinkedList());
            CalculateScore(winnerPart2).AsResult2();

            Report.End();
        }

        private static LinkedList<int> PlayGamePart2(LinkedList<int> player1, LinkedList<int> player2)
        {
            var signatureMemo = new HashSet<string>();
            while (player1.Count > 0 && player2.Count > 0)
            {
                if (IsLoop(player1, player2, signatureMemo))
                    return player1;

                var draw1 = Draw(player1);
                var draw2 = Draw(player2);
                var canRecurse1 = draw1 <= player1.Count;
                var canRecurse2 = draw2 <= player2.Count;

                bool player1winsRound;
                if (canRecurse1 && canRecurse2)
                    player1winsRound = PlaySubGame(player1, player2, draw1, draw2);
                else
                    player1winsRound = draw1 > draw2;

                if (player1winsRound)
                {
                    player1.AddLast(draw1);
                    player1.AddLast(draw2);
                }
                else
                {
                    player2.AddLast(draw2);
                    player2.AddLast(draw1);
                }
            }

            var winner = player1.Count > player2.Count
                ? player1
                : player2;
            return winner;
        }

        private static bool IsLoop(LinkedList<int> player1, LinkedList<int> player2, HashSet<string> signatureMemo)
        {
            var loop1 = CheckSignature(signatureMemo, "1", player1);
            var loop2 = CheckSignature(signatureMemo, "2", player2);
            var isLoop = loop1 || loop2;
            return isLoop;
        }

        private static bool PlaySubGame(LinkedList<int> player1, LinkedList<int> player2, int draw1, int draw2)
        {
            var subPlayer1 = player1.Take(draw1).ToLinkedList();
            var subPlayer2 = player2.Take(draw2).ToLinkedList();
            var subWinner = PlayGamePart2(subPlayer1, subPlayer2);
            return subPlayer1 == subWinner;
        }

        private static bool CheckSignature(HashSet<string> signatureMemo, string playerId, LinkedList<int> player)
        {
            var key = playerId + player.ToCommaString();
            if (signatureMemo.Contains(key))
            {
                return true;
            }
            signatureMemo.Add(key);
            return false;
        }

        private static LinkedList<int> PlayGamePart1(LinkedList<int> player1, LinkedList<int> player2)
        {
            while (player1.Count > 0 && player2.Count > 0)
            {
                var draw1 = Draw(player1);
                var draw2 = Draw(player2);

                if (draw1 > draw2)
                {
                    player1.AddLast(draw1);
                    player1.AddLast(draw2);
                }
                else
                {
                    player2.AddLast(draw2);
                    player2.AddLast(draw1);
                }
            }

            var winner = player1.Count > player2.Count ? player1 : player2;
            return winner;
        }

        private static long CalculateScore(LinkedList<int> winnersDeck)
        {
            return Enumerable.Range(1, winnersDeck.Count).Reverse()
                            .Zip(winnersDeck)
                            .Select(pair => (long)pair.First * pair.Second)
                            .Sum();
        }

        private static int Draw(LinkedList<int> player)
        {
            var card = player.First;
            player.Remove(card);
            return card.Value;
        }

        public static List<List<string>> LoadCardDecks(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Select(s => s.Trim())
                .GroupByLineSeperator()
                .ToList();
        }
    }
}
