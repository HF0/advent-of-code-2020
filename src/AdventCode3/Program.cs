using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode3
{
    class Program
    {
        private const char FreePath = '.';

        static void Main(string[] args)
        {
            Advent3();
        }

        private static void Advent3()
        {
            char[][] map = File.ReadAllLines("input.txt")
                .Select(e => e.ToCharArray()).ToArray();

            int solution1 = CountPath(map, 3, 1);
            Console.WriteLine("Solution 1 " + solution1);

            List<(int, int)> slopes = new List<(int, int)>
            {
                (1, 1), (3, 1), (5, 1), (7, 1), (1, 2)
            };
            int solution2 = slopes
                .Select(slope => CountPath(map, slope.Item1, slope.Item2))
                .Aggregate((a, b) => a * b);
            Console.WriteLine("Solution 2 " + solution2);
        }

        private static int CountPath(char[][] map, int right, int down)
        {
            int height = map.Length;
            int width = map[0].Length;

            int x = 0;
            int treesFound = 0;
            for (int y = 0; y < height; y += down)
            {
                treesFound += CountPosition(map, x, y);
                x = (x + right) % width;
            }
            return treesFound;
        }

        private static int CountPosition(char[][] map, int x, int y)
        {
            return map[y][x].Equals(FreePath) ? 0 : 1;
        }
    }
}
