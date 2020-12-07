using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode
{
    class Program
    {
        private const int Goal = 2020;
        static void Main(string[] args)
        {
            Advent11();
            Advent12();
        }

        private static void Advent11()
        {
            var lines = File.ReadAllLines("input1.txt");
            Dictionary<int, bool> dict = new Dictionary<int, bool>();
            var numbers = lines.Select(e => int.Parse(e));
            foreach (int number in numbers)
            {
                int otherNumber = Goal - number;
                if (dict.ContainsKey(otherNumber))
                {
                    int solution = otherNumber * number;
                    Console.WriteLine("Solution " + solution);
                    return;
                }
                dict.Add(number, true);
            }
        }

        private static void Advent12()
        {
            var lines = File.ReadAllLines("input1.csv");
            Dictionary<int, bool> dict1 = new Dictionary<int, bool>();
            Dictionary<int, List<int>> dict2 = new Dictionary<int, List<int>>();
            var numbers = lines.Select(e => int.Parse(e));
            foreach (int number in numbers)
            {
                int otherNumber = Goal - number;
                if (dict2.ContainsKey(otherNumber))
                {
                    var n = dict2[otherNumber];
                    int solution = number * n[0] * n[1];
                    Console.WriteLine("Solution " + solution);
                    return;
                }

                foreach (var number1 in dict1)
                {
                    int firstNumber = number1.Key;
                    int secondNumber = number;
                    int twoNumbers = firstNumber + secondNumber;
                    if (!dict2.ContainsKey(twoNumbers))
                    {
                        dict2.Add(twoNumbers, new List<int> { firstNumber, secondNumber });
                    }
                }
                dict1.Add(number, true);
            }
        }
    }
}
