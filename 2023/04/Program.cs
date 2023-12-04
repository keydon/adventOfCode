using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record Card
    {
        public int Id { get; set; }
        public List<int> Winners { get; set; }

        public List<int> Hand { get; set; }
        public int Copies { get; set; } = 1;

    }
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var cards = LoadCards("input.txt");
            //foos = LoadFoos("sample.txt");

            cards.Select(c => {
                var winningNums = c.Hand.Intersect(c.Winners).Count();
                ApplyCopies(winningNums, c.Id, cards, c.Copies);
                return c.Copies;
            })
            .Sum().AsResult2();


            Report.End();
        }

        private static void ApplyCopies(int times, int startCardId, List<Card> cards, int additionalCopies)
        {
            cards.SkipWhile(f => f.Id != startCardId).Skip(1)
            .Take(times)
            .ForEach(f => f.Copies += additionalCopies);
        }

        public static List<Card> LoadCards(string inputTxt)
        {
            var cards = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s => s.ParseRegex(@"^Card (.+): (.+) \| (.+)$", m => new Card()
                {
                   Id = int.Parse(m.Groups[1].Value),
                   Winners = m.Groups[2].Value.Splizz(" ").Select(int.Parse).ToList(),
                   Hand = m.Groups[3].Value.Splizz(" ").Select(int.Parse).ToList(),
                }))
                .ToList()
            ;

            return cards;
        }
    }
}
