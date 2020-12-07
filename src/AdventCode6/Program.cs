using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode6
{
    class Program
    {
        static void Main(string[] args)
        {
            var groupAnswers = File.ReadAllText("input.txt")
                .Split("\n\n")
                .Select(e => e.Split("\n", StringSplitOptions.RemoveEmptyEntries));
            int solution1 = groupAnswers
                .Select(text => CountDifferentAnswers(text))
                .Sum();
            Console.WriteLine($"Solution 1: {solution1}");

            int solution2 = groupAnswers
                .Select(text => CountAllSameAnswers(text))
                .Sum();
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static int CountAllSameAnswers(string[] lines)
        {
            int numPeopleInGroup = lines.Length;
            var charCount = lines
                .SelectMany(e => e)
                .GroupBy(e => e)
                .ToDictionary(e => e.Key, e => e.Count());
            return charCount
                .Where(e => e.Value == numPeopleInGroup)
                .Count();
        }
        private static int CountAllSameAnswers2(string[] lines)
        {
            int numPeopleInGroup = lines.Length;
            var answers = lines.SelectMany(e => e);
            Dictionary<char, int> charCount = new Dictionary<char, int>();
            foreach (var answer in answers)
            {
                charCount[answer] = charCount.ContainsKey(answer) ?
                        charCount[answer] + 1 :
                        1;
            }
            int a = charCount
                .Where(e => e.Value == numPeopleInGroup)
                .Count();
            return a;
        }

        private static int CountDifferentAnswers(string[] lines)
        {
            return lines
                .SelectMany(e => e)
                .Distinct()
                .Count();
        }

        private static int CountDifferentAnswers2(string[] lines)
        {
            var answers = lines.SelectMany(e => e);
            Dictionary<char, bool> countAnswers = new Dictionary<char, bool>();
            foreach (char a in answers)
            {
                if (!countAnswers.ContainsKey(a))
                {
                    countAnswers.Add(a, true);
                }
            }
            return countAnswers.Count();
        }

        private static int CountDifferentAnswers3(string[] lines)
        {
            var answers = lines.SelectMany(e => e);
            var charCount = lines
                .SelectMany(e => e)
                .GroupBy(e => e)
                .ToDictionary(e => e.Key, e => e.Count());
            return charCount.Count();
        }
    }
}
