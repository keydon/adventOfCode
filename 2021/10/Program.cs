using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    class Program
    {
        private static readonly Dictionary<char, char> Paranthesis = new()
        {
                { '(', ')' },
                { '[', ']' },
                { '{', '}' },
                { '<', '>' }
            };
        private static readonly Dictionary<char, int> IllegalCharScores = new()
        {
                { ')', 3 },
                { ']', 57 },
                { '}', 1197 },
                { '>', 25137 }
        };
        private static readonly Dictionary<char, int> AutoCompleteScores = new(){
              {'(', 1},
              {'[', 2},
              {'{', 3},
              {'<', 4}
            };

        static void Main(string[] args)
        {
            Report.Start();
            var lines = LoadParanthesis("input.txt");
            //lines = LoadParanthesis("sample.txt" +"");
        
            var (incompleteLines, illigalChars) = Analyze(lines);

            illigalChars.Select(chr => IllegalCharScores[chr]).Sum().AsResult1();

            var scores = AutoComplete(incompleteLines);
            var middleScore = scores[scores.Count / 2];
            middleScore.AsResult2();

            Report.End();
        }

        private static (List<List<char>> incompleteLines, List<char> illigalChars) Analyze(List<char[]> lines)
        {
            var incompleteLines = new List<List<char>>();
            var illigalChars = new List<char>();
            lines.ForEach(line => AnalyzeLine(line, incompleteLines, illigalChars));
            return (incompleteLines, illigalChars);
        }
        
        private static void AnalyzeLine(char[] line, List<List<char>> incompleteLines, List<char> illigalChars){
            var ll = new LinkedList<char>(line);

            var node = ll.First;
            while(node.Next != null) {
                var previous = node.Previous;
                var open = node;
                var close = node.Next;
                if(Paranthesis.TryGetValue(open.Value, out char expectedClose)){
                    if(close.Value == expectedClose){
                        ll.Remove(open);
                        ll.Remove(close);
                        node = previous;
                        continue;
                    }
                } 
                node = node.Next;
            }

            var reveresdParanthesis = Paranthesis.Select(p => p).ToDictionary(k => k.Value, v => v.Key);

            node = ll.First;
            while(node != null) {
                var close = node;
                if (reveresdParanthesis.TryGetValue(close.Value, out char expectedOpen)){
                    illigalChars.Add(close.Value);
                    return;
                } 
                node = node.Next;
            }

            incompleteLines.Add(ll.ToList());
        }

        private static List<long> AutoComplete(List<List<char>> incompleteLines)
        {
            var scores = incompleteLines
                .Select(list => list.AsEnumerable())
                .Select(enumerable => enumerable.Reverse())
                .Select(l => l.Aggregate(0L, (intermediateScore, c) =>
                {
                    var previous = intermediateScore * 5L;
                    return previous + AutoCompleteScores[c];
                }))
                .OrderBy(score => score).ToList();

            return scores;
        }

        public static List<char[]> LoadParanthesis(string inputTxt)
        {
            var foos = File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToCharArray())
                .ToList();

            return foos;
        }
    }
}
