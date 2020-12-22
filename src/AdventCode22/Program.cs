using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode22
{
    class Program
    {
        static void Main(string[] args)
        {
            var sections = File.ReadAllText("input.txt").Split("\n\n");
            var deck1a = ParseDeck(sections[0]);
            var deck2a = ParseDeck(sections[1]);

            Queue<int> winnerDeck = PlayGame(deck1a, deck2a);
            int solution1 = CalculateScore(winnerDeck);
            Console.WriteLine($"Solution 1: {solution1}");

            var deck1b = ParseDeck(sections[0]);
            var deck2b = ParseDeck(sections[1]);
            var (_, deck) = PlayGame2(deck1b, deck2b);
            int solution2 = CalculateScore(deck);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static int CalculateScore(Queue<int> winnerDeck)
        {
            int numCards = winnerDeck.Count();
            return winnerDeck
                .Select((e, i) => e * (numCards - i))
                .Sum();
        }

        private static (bool winner1, Queue<int> deck) PlayGame2(Queue<int> deck1, Queue<int> deck2)
        {
            ISet<Queue<int>> deck1History = new HashSet<Queue<int>>();
            ISet<Queue<int>> deck2History = new HashSet<Queue<int>>();
            do
            {
                if (deck1History.Any(d => d.SequenceEqual(deck1)) ||
                deck2History.Any(d => d.SequenceEqual(deck2)))
                {
                    return (true, deck1);
                }
                deck1History.Add(new Queue<int>(deck1));
                deck2History.Add(new Queue<int>(deck2));

                var top1 = deck1.Dequeue();
                var top2 = deck2.Dequeue();
                bool roundWinner1;
                if (top1 <= deck1.Count() && top2 <= deck2.Count())
                {
                    var roundResult = PlayGame2(new Queue<int>(deck1.Take(top1)),
                        new Queue<int>(deck2.Take(top2)));
                    roundWinner1 = roundResult.winner1;
                }
                else
                {
                    roundWinner1 = top1 > top2;
                }
                if (roundWinner1)
                {
                    deck1.Enqueue(top1);
                    deck1.Enqueue(top2);
                }
                else
                {
                    deck2.Enqueue(top2);
                    deck2.Enqueue(top1);
                }
            } while (deck1.Count() > 0 && deck2.Count() > 0);
            bool winner1 = deck2.Count() == 0;
            var winnerDeck = winner1 ? deck1 : deck2;
            return (winner1, winnerDeck);
        }

        private static Queue<int> PlayGame(Queue<int> deck1, Queue<int> deck2)
        {
            do
            {
                var top1 = deck1.Dequeue();
                var top2 = deck2.Dequeue();
                if (top1 > top2)
                {
                    deck1.Enqueue(top1);
                    deck1.Enqueue(top2);
                }
                else
                {
                    deck2.Enqueue(top2);
                    deck2.Enqueue(top1);
                }
            } while (deck1.Count() > 0 && deck2.Count() > 0);
            var winnerDeck = deck1.Count() == 0 ? deck2 : deck1;
            return winnerDeck;
        }

        private static Queue<int> ParseDeck(string section)
        {
            var playerDeck = section
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(e => int.Parse(e));
            return new Queue<int>(playerDeck);
        }
    }
}
