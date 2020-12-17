using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCode15
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "2,1,10,11,0,6";
            IEnumerable<int> initialNumbers = input.Split(',').Select(e => int.Parse(e));

            int solution1 = CalculateSolution(initialNumbers, 2020);
            Console.WriteLine($"Solution 1: {solution1}");

            int solution2 = CalculateSolution(initialNumbers, 30000000);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static Dictionary<int, (int turn, bool firstSpoken)> BuildInitialState(IEnumerable<int> initialNumbers)
        {
            return initialNumbers
                .Select((v, i) => (v, i))
                .ToDictionary(k => k.v, k => (k.i + 1, true));
        }

        private static int CalculateSolution(IEnumerable<int> initialNumbers, int goalTurn)
        {
            Dictionary<int, (int turn, bool firstSpoken)> state = BuildInitialState(initialNumbers);
            int lastSpokenTurn = state.Count();
            int lastNumberSpoken = initialNumbers.Last();
            int currentTurn = lastSpokenTurn + 1;

            while (lastSpokenTurn < goalTurn)
            {
                int currentSpokenNumber;
                var infoLastNumber = state[lastNumberSpoken];
                if (infoLastNumber.firstSpoken)
                {
                    currentSpokenNumber = 0;
                }
                else
                {
                    currentSpokenNumber = lastSpokenTurn - infoLastNumber.turn;
                    infoLastNumber.turn = lastSpokenTurn;
                    state[lastNumberSpoken] = infoLastNumber;
                }
                UpdateCurrentNumber(state, currentTurn, currentSpokenNumber);

                lastNumberSpoken = currentSpokenNumber;
                lastSpokenTurn++;
                currentTurn++;
            }
            return lastNumberSpoken;
        }

        private static void UpdateCurrentNumber(Dictionary<int, (int turn, bool firstSpoken)> state, int currentTurn, int currentSpokenNumber)
        {
            (int turn, bool firstSpoken) spokenInfo;
            if (state.ContainsKey(currentSpokenNumber))
            {
                spokenInfo = state[currentSpokenNumber];
                spokenInfo.firstSpoken = false;
            }
            else
            {
                spokenInfo = (currentTurn, true);
            }
            state[currentSpokenNumber] = spokenInfo;
        }
    }
}
