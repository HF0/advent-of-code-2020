using System;
using System.IO;
using System.Linq;

namespace AdventCode11
{
    class Program
    {
        private const char EmptySeat = 'L';
        private const char OccupiedSeat = '#';
        private delegate char UpdateSeatFunction(char[][] grid, (int x, int y) seatPosition);
        static void Main(string[] args)
        {
            var grid = File.ReadAllLines("input.txt")
                .Select(e => e.ToCharArray())
                .ToArray();

            char[][] destination1 = CalculateEvolveSeats(grid, UpdateSeatProblem1);
            int solution1 = CountOccupiedSeats(destination1);
            Console.WriteLine($"Solution 1: {solution1}");

            char[][] destination2 = CalculateEvolveSeats(grid, UpdateSeatProblem2);
            int solution2 = CountOccupiedSeats(destination2);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static char[][] CalculateEvolveSeats(char[][] grid, UpdateSeatFunction updateSeat)
        {
            int width = grid[0].Length;
            int height = grid.Length;

            char[][] source = Clone(grid);
            char[][] destination = CreateGrid(width, height);
            while (UpdateGrid(source, destination, updateSeat))
            {
                char[][] tmp = source;
                source = destination;
                destination = tmp;
            }
            return destination;
        }

        private static char[][] Clone(char[][] grid) => grid.Select(s => s.ToArray()).ToArray();

        private static char[][] CreateGrid(int width, int height)
        {
            var otherGrid = new char[height][];
            for (int i = 0; i < height; i++)
            {
                otherGrid[i] = new char[width];
            }
            return otherGrid;
        }

        private static void Show(char[][] grid)
        {
            Console.WriteLine();
            foreach (var row in grid)
            {
                Console.WriteLine(string.Join("", row));
            }
        }

        private static int CountOccupiedSeats(char[][] grid) => grid.Select(e => e.Count(s => s.Equals(OccupiedSeat))).Sum();

        private static bool UpdateGrid(char[][] grid, char[][] gridOutput, UpdateSeatFunction updateSeatFunction)
        {
            int width = grid[0].Length;
            int height = grid.Length;
            bool changed = false;
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    char currentSeatStatus = grid[x][y];
                    gridOutput[x][y] = updateSeatFunction(grid, (x, y));
                    changed = changed || !gridOutput[x][y].Equals(currentSeatStatus);
                }
            }
            return changed;
        }

        private static char UpdateSeatProblem1(char[][] grid, (int x, int y) position)
        {
            char place = grid[position.x][position.y];
            return place switch
            {
                EmptySeat => AdjancentOccupiedSeats(position, grid) == 0 ? OccupiedSeat : place,
                OccupiedSeat => AdjancentOccupiedSeats(position, grid) >= 4 ? EmptySeat : place,
                _ => grid[position.x][position.y],
            };
        }

        private static char UpdateSeatProblem2(char[][] grid, (int x, int y) position)
        {
            char place = grid[position.x][position.y];
            return place switch
            {
                EmptySeat => EyeOccupiedSeat(position, grid) == 0 ? OccupiedSeat : place,
                OccupiedSeat => EyeOccupiedSeat(position, grid) >= 5 ? EmptySeat : place,
                _ => grid[position.x][position.y],
            };
        }

        private static int AdjancentOccupiedSeats((int x, int y) position, char[][] grid)
        {
            int count = 0;
            for (int x = position.x - 1; x <= position.x + 1; x++)
            {
                for (int y = position.y - 1; y <= position.y + 1; y++)
                {
                    if (ValidPosition((x, y), grid) &&
                        (!position.x.Equals(x) || !position.y.Equals(y)) &&
                        grid[x][y].Equals(OccupiedSeat))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private static int EyeOccupiedSeat((int x, int y) position, char[][] grid)
        {
            int count = 0;
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if ((offsetX != 0 || offsetY != 0) &&
                        CanSeeOccupiedSeat((offsetX, offsetY), position, grid))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private static bool CanSeeOccupiedSeat((int x, int y) offset, (int x, int y) position, char[][] grid)
        {
            position.x += offset.x;
            position.y += offset.y;
            while (ValidPosition(position, grid))
            {
                char seat = grid[position.x][position.y];
                if (seat.Equals(OccupiedSeat))
                {
                    return true;
                }
                if (seat.Equals(EmptySeat))
                {
                    return false;
                }
                position.x += offset.x;
                position.y += offset.y;
            }
            return false;
        }

        private static bool ValidPosition((int x, int y) position, char[][] grid)
        {
            int width = grid[0].Length;
            int height = grid.Length;
            return position.x >= 0 && position.x < height && position.y >= 0 && position.y < width;
        }
    }
}
