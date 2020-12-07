using System;
using System.IO;
using System.Linq;

namespace AdventCode2
{
    class Program
    {
        static void Main(string[] args)
        {
            Advent2();
        }

        private static void Advent2()
        {
            var objects = File.ReadAllLines("input2.txt")
                .Select(e => e.Trim().Split(new char[] { ' ', ':', '-' }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries))
                .Select(e => new
                {
                    Num1 = int.Parse(e[0]),
                    Num2 = int.Parse(e[1]),
                    Letter = e[2][0],
                    Word = e[3]
                });

            int solution1 = objects.Where(o => CheckPasswordPolicy(o.Num1, o.Num2, o.Letter, o.Word)).Count();
            Console.WriteLine("Solution1: " + solution1);

            int solution2 = objects.Where(o => CheckPasswordPolicy2(o.Num1, o.Num2, o.Letter, o.Word)).Count();
            Console.WriteLine("Solution2: " + solution2);
        }

        private static bool CheckPasswordPolicy(int min, int max, char letter, string word)
        {
            int letterOcurrences = word.Where(c => c.Equals(letter)).Count(); ;
            return min <= letterOcurrences && letterOcurrences <= max;
        }

        private static bool CheckPasswordPolicy2(int pos1, int pos2, char letter, string word)
        {
            char c1 = word[pos1-1];
            char c2 = word[pos2-1];
            return (c1.Equals(letter)) ^ (c2.Equals(letter));
        }
    }
}
