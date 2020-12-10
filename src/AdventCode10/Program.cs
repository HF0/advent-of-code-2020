using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode10
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = File.ReadAllLines("input.txt")
                .Select(e => int.Parse(e))
                .OrderBy(e => e)
                .ToList();
            int solution1 = GetSolution1(numbers);
            Console.WriteLine($"Solution 1: {solution1}");

            long solution2 = GetSolution2(numbers);
            Console.WriteLine($"Solution 1: {solution2}");
        }

        private static long GetSolution2(List<int> numbers)
        {
            Dictionary<int, long> calculatedConfigurations = new Dictionary<int, long>();
            calculatedConfigurations.Add(numbers.Last(), 1);

            Stack<int> configurationToCalculate = new Stack<int>();
            configurationToCalculate.Push(numbers.First());
            while (configurationToCalculate.Count() > 0)
            {
                var conf = configurationToCalculate.Peek();
                if (calculatedConfigurations.ContainsKey(conf))
                {
                    configurationToCalculate.Pop();
                    continue;
                }

                var compatible = GetCompatiblePlugs(conf, numbers);
                var canCalculate = compatible.All(e => calculatedConfigurations.ContainsKey(e));
                if (canCalculate)
                {
                    calculatedConfigurations.Add(conf, compatible.Select(e => calculatedConfigurations[e]).Sum());
                    configurationToCalculate.Pop();
                }
                else
                {
                    AddAll(configurationToCalculate, compatible);
                }
            }
            return calculatedConfigurations[0];
        }

        private static void AddAll(Stack<int> configurationToCalculate, IEnumerable<int> compatible)
        {
            foreach (var comp in compatible)
            {
                configurationToCalculate.Push(comp);
            }
        }

        private static IEnumerable<int> GetCompatiblePlugs(int conf, List<int> numbers)
        {
            int index = numbers.BinarySearch(conf);
            return numbers.Skip(index + 1).TakeWhile(e => e - conf <= 3);
        }

        private static int GetSolution1(List<int> numbers)
        {
            numbers.Insert(0, 0);
            numbers.Add(numbers.Last() + 3);

            var difference = numbers.Zip(numbers.Skip(1))
                .Select(e => e.Second - e.First);

            int dif1 = difference.Count(e => e == 1);
            int dif3 = difference.Count(e => e == 3);
            int solution = dif1 * dif3;
            return solution;
        }
    }
}
