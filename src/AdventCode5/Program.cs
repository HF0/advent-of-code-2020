using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode5
{
    class Program
    {
        static void Main(string[] args)
        {
            var seatIds = File.ReadAllLines("input.txt")
                            .Select(l => GetRowId3(l))
                            .OrderBy(e => e)
                            .ToList();

            int max = seatIds.Max();
            Console.WriteLine($"Solution 1: {max}");

            int missingNumber = FindMissingNumber(seatIds);
            int missingNumber2 = FindMissingNumberBinarySearch(seatIds);
            Console.WriteLine($"Solution2 : {missingNumber} {missingNumber2}");
        }

        private static int FindMissingNumber(List<int> orderedSeatIds)
        {
            bool found = false;
            int missingNumber = -1;
            for (int i = 1; i < orderedSeatIds.Count() && !found; i++)
            {
                if (orderedSeatIds[i] != (orderedSeatIds[i - 1] + 1))
                {
                    missingNumber = orderedSeatIds[i] - 1;
                    found = true;
                }
            }
            return missingNumber;
        }

        private static int FindMissingNumberBinarySearch(List<int> orderedSeatIds)
        {
            int offset = orderedSeatIds[0];
            int i = 0;
            int j = orderedSeatIds.Count() - 1;
            while (i < j)
            {
                Console.WriteLine($"Binary search {i} {j}");
                int mid = (i + j) / 2;
                int value = orderedSeatIds[mid];
                // That value in position i should be: (value of first element called offset) + i
                int expectedValue = offset + mid;
                if (value == expectedValue)
                {
                    i = mid + 1;
                }
                else
                {
                    j = mid;
                }
            }
            Console.WriteLine($"Binary search {i} {j}");
            int missingValuePlus1 = orderedSeatIds[i];
            return missingValuePlus1 - 1;
        }

        private static (int row, int column) GetRowColumn(string encodedSeat)
        {
            string rowPart = encodedSeat[0..^3];
            string columnPart = encodedSeat[^3..];
            int row = GetSeat(rowPart, 'F', 'B');
            int column = GetSeat(columnPart, 'L', 'R');
            return (row, column);
        }

        private static int GetSeat(string value, char lowerChar, char highChar)
        {
            // value is a binary number represented as a string, value goes from 0 to 2^length-1
            int min = 0;
            int max = IntPow(2, value.Length) - 1;
            foreach (var c in value)
            {
                int mid = (min + max) / 2;

                if (lowerChar.Equals(c))
                {
                    max = mid;
                }
                else if (highChar.Equals(c))
                {
                    min = mid + 1;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            return min;
        }

        private static int GetRowId2(string encodedString)
        {
            int positionValue = 1;
            int value = 0;
            for(int i=encodedString.Length-1; i >=0; i--)
            {
                var c = encodedString[i];
                value += BinaryValue(c) * positionValue;
                positionValue *= 2;
            }
            return value;
        }

        private static int GetRowId3(string encodedString)
        {
            encodedString = encodedString.Replace('F', '0');
            encodedString = encodedString.Replace('L', '0');
            encodedString = encodedString.Replace('B', '1');
            encodedString = encodedString.Replace('R', '1');
            return Convert.ToInt32(encodedString, 2);
        }

        private static int BinaryValue(char c)
        {
            switch (c)
            {
                case 'L':
                case 'F':
                    return 0;
                case 'B':
                case 'R':
                    return 1;
                default:
                    throw new ArgumentException();
            };
        }

        private static int GetRowId(string encodedString)
        {
            (int row, int column) = GetRowColumn(encodedString);
            return GetRowId(row, column);
        }

        private static int GetRowId(int row, int column) => row * 8 + column;
        private static int IntPow(int bas, int exp)
        {
            return Enumerable.Repeat(bas, exp).Aggregate(1, (a, b) => a * b);
        }
    }
}
