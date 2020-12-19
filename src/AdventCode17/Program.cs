using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace AdventCode17
{
    class Program
    {
        private const char Active = '#';
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            int solution1 = FindSolution(lines, 3, 6);
            Console.WriteLine($"Solution 1: {solution1}");

            int solution2 = FindSolution(lines, 4, 6);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static int FindSolution(string[] lines, int dimensions, int iterations)
        {
            var activePositions = lines
                .SelectMany((cubeRow, columnIndex) => CreateCubes(cubeRow.ToArray(), columnIndex, dimensions))
                .Where(k => k.active)
                .Select(k => k.position)
                .ToHashSet(new MyEqualityComparer());
            for (int i = 0; i < iterations; i++)
            {
                UpdateWholeState(activePositions);
            }
            return activePositions.Count();
        }

        private static IEnumerable<(IEnumerable<int> position, bool active)> CreateCubes(char[] cubeRow, int columnIndex, int dimensions)
            => cubeRow.Select((c, rowIndex) => (CreateDimensionPosition(rowIndex, columnIndex, dimensions), c.Equals(Active)));

        private static IEnumerable<int> CreateDimensionPosition(int row, int column, int dimensions)
        {
            return new[] { row, column }.Concat(Enumerable.Repeat(0, dimensions - 2));
        }

        private static void UpdateWholeState(HashSet<IEnumerable<int>> activePositions)
        {
            Dictionary<IEnumerable<int>, int> activeNeighbors = new Dictionary<IEnumerable<int>, int>(new MyEqualityComparer());
            foreach (var position in activePositions)
            {
                foreach (var activePosition in GetNeighboursIncludingMe(position))
                {
                    activeNeighbors[activePosition] = activeNeighbors.ContainsKey(activePosition) ? activeNeighbors[activePosition] + 1 : 1;
                }
            }
            foreach (var activePosition in activeNeighbors)
            {
                if (GetNextValue(activePositions.Contains(activePosition.Key), activePosition.Value))
                {
                    activePositions.Add(activePosition.Key);
                }
                else
                {
                    activePositions.Remove(activePosition.Key);
                }
            }
        }

        private static bool GetNextValue(bool cubeActive, int activeNeighboursIncludingMe)
        {
            return cubeActive ?
                activeNeighboursIncludingMe - 1 == 2 || activeNeighboursIncludingMe - 1 == 3 :
                activeNeighboursIncludingMe == 3;
        }

        private static IEnumerable<IEnumerable<int>> GetNeighboursIncludingMe(IEnumerable<int> position)
        {
            List<List<int>> alreadyCalculated = new List<List<int>>
            {
                new List<int>()
            };
            foreach (var dim in position)
            {
                List<List<int>> newResult = new List<List<int>>();
                foreach (var previousList in alreadyCalculated)
                {
                    for (int i = dim - 1; i <= dim + 1; i++)
                    {
                        var newList = previousList.ToList();
                        newList.Add(i);
                        if (newList.Count() == position.Count())
                        {
                            yield return newList;
                        }
                        else
                        {
                            newResult.Add(newList);
                        }
                    }
                }
                alreadyCalculated = newResult;
            }
        }

        // to use ienumerable<int> as dictionary key
        class MyEqualityComparer : IEqualityComparer<IEnumerable<int>>
        {
            public bool Equals([AllowNull] IEnumerable<int> x, [AllowNull] IEnumerable<int> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode([DisallowNull] IEnumerable<int> obj)
            {
                int hash = 1;
                foreach (var o in obj)
                {
                    hash = hash * 31 + o.GetHashCode();
                }
                return hash;
            }
        }
    }
}
