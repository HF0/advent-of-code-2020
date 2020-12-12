using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode12
{
    class Program
    {
        private delegate void ProcessInstructionDelegate((char action, int value) instruction, ref (int x, int y) pos, ref (int x, int y) direction);
        private static readonly (int, int) InitialPosition = (0, 0);
        static void Main(string[] args)
        {
            var instructions = File.ReadAllLines("input.txt")
                .Select(e => ParseInstruction(e));

            // n/s e/w
            (int, int) waypoint = (0, 1);
            (int, int) position = InitialPosition;
            int solution1 = GetSolution(instructions, ref waypoint, ref position, ProcessInstructionProblem1);
            Console.WriteLine($"Solution 1: {solution1}");

            (int, int) waypoint2 = (1, 10);
            (int, int) position2 = InitialPosition;
            int solution2 = GetSolution(instructions, ref waypoint2, ref position2, ProcessInstructionProblem2);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static int GetSolution(IEnumerable<(char, int)> instructions, ref (int, int) waypoint, ref (int, int) position,
            ProcessInstructionDelegate processInstruction)
        {
            foreach (var instruction in instructions)
            {
                processInstruction(instruction, ref position, ref waypoint);
            }
            return ManhattanDistance(position, InitialPosition);
        }


        private static void ProcessInstructionProblem1((char action, int value) instruction, ref (int x, int y) pos, ref (int x, int y) direction)
        {
            switch (instruction.action)
            {
                case 'N':
                    pos = AdvancePosition(instruction.value, pos, (1, 0));
                    break;
                case 'S':
                    pos = AdvancePosition(instruction.value, pos, (-1, 0));
                    break;
                case 'E':
                    pos = AdvancePosition(instruction.value, pos, (0, 1));
                    break;
                case 'W':
                    pos = AdvancePosition(instruction.value, pos, (0, -1));
                    break;
                case 'L':
                    direction = ChangeDirection(direction, -1 * instruction.value);
                    break;
                case 'R':
                    direction = ChangeDirection(direction, instruction.value);
                    break;
                case 'F':
                    pos = AdvancePosition(instruction.value, pos, direction);
                    break;
                default:
                    throw new ArgumentException();
            };
        }

        private static void ProcessInstructionProblem2((char action, int value) instruction, ref (int x, int y) pos, ref (int x, int y) waypoint)
        {
            switch (instruction.action)
            {
                case 'N':
                    waypoint = AdvancePosition(instruction.value, waypoint, (1, 0));
                    break;
                case 'S':
                    waypoint = AdvancePosition(instruction.value, waypoint, (-1, 0));
                    break;
                case 'E':
                    waypoint = AdvancePosition(instruction.value, waypoint, (0, 1));
                    break;
                case 'W':
                    waypoint = AdvancePosition(instruction.value, waypoint, (0, -1));
                    break;
                case 'L':
                    waypoint = ChangeDirection(waypoint, -1 * instruction.value);
                    break;
                case 'R':
                    waypoint = ChangeDirection(waypoint, instruction.value);
                    break;
                case 'F':
                    pos = AdvancePosition(instruction.value, pos, waypoint);
                    break;
                default:
                    throw new ArgumentException();
            };
        }

        private static (int x, int y) AdvancePosition(int distance, (int x, int y) pos, (int x, int y) waypoing)
        {
            pos.x += waypoing.x * distance;
            pos.y += waypoing.y * distance;
            return pos;
        }

        private static (int x, int y) ChangeDirection((int x, int y) direction, int value)
        {
            if (value % 90 != 0)
            {
                throw new ArgumentException();
            }
            value /= 90;
            value %= 4;
            if (value == 0)
            {
                return direction;
            }
            (int x, int y) newDirection;
            switch (value)
            {
                case 1:
                case -3:
                    newDirection = (direction.y, direction.x);
                    if (direction.y != 0)
                    {
                        newDirection.x *= -1;
                    }
                    break;
                case 3:
                case -1:
                    newDirection = (direction.y, direction.x);
                    if (direction.x != 0)
                    {
                        newDirection.y *= -1;
                    }
                    break;
                case 2:
                case -2:
                    newDirection = (direction.x * -1, direction.y * -1);
                    break;
                default:
                    throw new ArgumentException();
            }
            return newDirection;
        }

        private static (char, int) ParseInstruction(string e) => (e[0], int.Parse(e[1..]));
        private static int ManhattanDistance((int x, int y) p1, (int x, int y) p2) => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);

    }
}
