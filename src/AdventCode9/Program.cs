using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode9
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = File.ReadAllLines("input.txt")
                .Select(e => int.Parse(e));

            int solution1a = GetSolution1(numbers);
            Console.WriteLine($"Solution 1: {solution1a}");

            var listNumber = FindContiguousSum(solution1a, numbers);
            int solution2a = listNumber.Max() + listNumber.Min();
            Console.WriteLine($"Solution 2: {solution2a}");
        }

        private static int GetSolution1(IEnumerable<int> numbers)
        {
            var preamble = new Queue<int>(numbers.Take(25));
            var remainingNumbers = numbers.Skip(25);
            int solution1 = 0;
            foreach (var n in remainingNumbers)
            {
                if (!CheckSum(n, preamble))
                {
                    solution1 = n;
                    break;
                }
                preamble.Dequeue();
                preamble.Enqueue(n);
            }
            return solution1;
        }

        private static IEnumerable<int> FindContiguousSum(int solution1a, IEnumerable<int> numbers)
        {
            int sum = 0;
            Queue<int> result = new Queue<int>();
            foreach(var n in numbers)
            {
                while (sum > solution1a && result.Count() > 0)
                {
                    int v = result.Dequeue();
                    sum -= v;
                }
                if (sum == solution1a)
                {
                    break;
                }
                    
                result.Enqueue(n);
                sum += n;
                if (sum == solution1a)
                {
                    break;
                }
            }
            return result;
        }

        private static bool CheckSum(int sumValue, Queue<int> preamble)
        {
            ISet<int> neededNumber = new HashSet<int>();
            foreach (var v in preamble)
            {
                if (neededNumber.Contains(v))
                {
                    return true;
                }
                else
                {
                    neededNumber.Add(sumValue - v);
                }
            }
            return false;
        }
    }
}
