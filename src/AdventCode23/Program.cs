using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCode23
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "389547612";
            var values = input
                .ToCharArray()
                .Select(e => int.Parse(e.ToString()))
                .ToList();

            Dictionary<int, int> cups1 = new Dictionary<int, int>();
            for (int i = 0; i < values.Count() - 1; i++)
            {
                cups1.Add(values[i], values[i + 1]);
            }
            cups1.Add(values[^1], values[0]);
            Dictionary<int, int> cups2 = new Dictionary<int, int>(cups1);

            Dictionary<int, int> cups = PlayGame(values[0], cups1, 100);
            string solution1 = CalculateSolution(cups);
            Console.WriteLine($"Solution 1: {solution1}");

            int maxValue = values.Max();
            int minValue = values.Min();

            cups2[values.Last()] = maxValue + 1;
            int lastValue = 1000000;
            for (int i = maxValue + 1; i < lastValue; i++)
            {
                cups2[i] = i + 1;
            }
            cups2[lastValue] = values[0];
            Dictionary<int, int> cupsState = PlayGame(values[0], cups2, 10000000);

            var next1 = cupsState[1];
            var next2 = cupsState[next1];
            long solution2 = (long)next1 * next2;
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static Dictionary<int, int> PlayGame(int firstCup, Dictionary<int, int> cups, int moves)
        {
            int lowestNumber = cups.Keys.Min();
            int highestNumber = cups.Keys.Max();

            int currentCupNode = firstCup;
            for (int i = 0; i < moves; i++)
            {
                currentCupNode = IterateGame(currentCupNode, cups, lowestNumber, highestNumber);
            }
            return cups;
        }

        private static LinkedListNode<int> NextCup(LinkedList<int> cups, LinkedListNode<int> currentCup) 
            => currentCup.Next ?? cups.First;

        private static int IterateGame(int currentCup,
            Dictionary<int, int> cups,
            int lowestNumber, int highestNumber)
        {
            var cup1 = cups[currentCup];
            var cup2 = cups[cup1];
            var cup3 = cups[cup2];
            cups[currentCup] = cups[cup3];

            int destinationCup = currentCup;
            do
            {
                destinationCup--;
                if (destinationCup < lowestNumber)
                {
                    destinationCup = highestNumber;
                }
            } while (destinationCup == cup1 || destinationCup == cup2 || destinationCup == cup3);

            int nextDestinationCup = cups[destinationCup];
            cups[destinationCup] = cup1;
            cups[cup3] = nextDestinationCup;
            currentCup = cups[currentCup];
            return currentCup;
        }

        private static string CalculateSolution(Dictionary<int, int> cups)
        {
            StringBuilder solution = new StringBuilder();

            var next = cups[1];
            do
            {
                solution.Append(next);
                next = cups[next];
            } while (next != 1);
            return solution.ToString();
        }
    }
}
