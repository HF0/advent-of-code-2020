using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode8
{
    class Program
    {
        private const string Nop = "nop";
        private const string Jmp = "jmp";
        private const string Acc = "acc";
        private static readonly Dictionary<string, string> ChangeableInstructions = new Dictionary<string, string>
        {
            { Jmp, Nop },
            { Nop, Jmp },
        };

        static void Main(string[] args)
        {
            var instructions = File.ReadAllLines("input.txt")
                .Select(e => ParseInstruction(e))
                .ToArray();

            (int accumulator, bool loop) = RunProgram(instructions);

            int solution1a = accumulator;
            Console.WriteLine($"Solution 1: {solution1a}");

            int solution2a = TestFixProgram(instructions); ;
            Console.WriteLine($"Solution 2: {solution2a}");
        }

        private static int TestFixProgram((string instruction, int value)[] instructions)
        {
            int lastInstructionChanged = 0;
            int accumulator;
            bool loop;
            do
            {
                lastInstructionChanged = FindNextChangeableInstruction(lastInstructionChanged, instructions);

                ToggleInstruction(ref instructions[lastInstructionChanged]);
                (accumulator, loop) = RunProgram(instructions);
                ToggleInstruction(ref instructions[lastInstructionChanged]);

            } while (loop);
            return accumulator;
        }

        private static void ToggleInstruction(ref (string opCode, int value) instruction) => 
            instruction.opCode = ChangeableInstructions[instruction.opCode];


        private static int FindNextChangeableInstruction(int fromInstructionPointer, (string instruction, int value)[] instructions)
        {
            do
            {
                fromInstructionPointer++;
            } while (!IsChangeable(instructions[fromInstructionPointer]));
            return fromInstructionPointer;
        }

        private static bool IsChangeable((string instruction, int value) p) => ChangeableInstructions.Keys.Contains(p.instruction);

        private static (int accumulator, bool loop) RunProgram((string opcode, int value)[] instructions)
        {
            int accumulator = 0;
            bool loop = false;
            int instructionPointer = 0;

            ISet<int> instructionExecuted = new HashSet<int>();
            while (instructionPointer < instructions.Length && !loop)
            {
                if (instructionExecuted.Contains(instructionPointer))
                {
                    loop = true;
                }
                else
                {
                    int instructionOffset = ExecuteInstruction(instructions[instructionPointer], ref accumulator);
                    instructionExecuted.Add(instructionPointer);
                    instructionPointer += instructionOffset;
                }
            }
            return (accumulator, loop);
        }

        private static int ExecuteInstruction((string opcode, int value) instruction, ref int accumulator)
        {
            int offset = 1;
            var (opcode, value) = instruction;
            switch (opcode)
            {
                case Nop:
                    break;
                case Acc:
                    accumulator += value;
                    break;
                case Jmp:
                    offset = value;
                    break;
                default:
                    throw new ArgumentException();
            };
            return offset;
        }

        private static (string instruction, int value) ParseInstruction(string e)
        {
            var tokens = e.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return (tokens[0], int.Parse(tokens[1]));
        }
    }
}
