using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode14
{
    class Program
    {
        static void Main(string[] args)
        {
            var instructions = File.ReadAllLines("input.txt")
                .Select(e => ParseInstruction(e))
                .ToList();

            Dictionary<ulong, ulong> memory = new Dictionary<ulong, ulong>();
            (ulong zeroMask, ulong oneMask) mask = (0xffffffff, 0);
            instructions.ForEach(instruction => ProcessInstruction1(instruction, memory, ref mask));
            ulong solution1 = memory.Values.Aggregate((a, b) => a + b);
            Console.WriteLine($"Solution 1 {solution1}");

            memory.Clear();
            (ulong orMask, IEnumerable<int> floatingIndexList) memoryMask = (0, Enumerable.Empty<int>());
            instructions.ForEach(instruction => ProcessInstruction2(instruction, memory, ref memoryMask));
            ulong solution2 = memory.Values.Aggregate((a, b) => a + b);
            Console.WriteLine($"Solution 2 {solution2}");
        }

        private static void WriteToMemory2(Dictionary<ulong, ulong> memory, ulong memoryAddress, ulong value,
            (ulong orMask, IEnumerable<int> floatingIndexList) memoryMask)
        {
            ulong memoryAddressWithoutFloating = memoryAddress | memoryMask.orMask;
            List<int> floatIndexList = memoryMask.floatingIndexList.ToList();
            var memoryAddresses = GenerateMemoryPositions(memoryAddressWithoutFloating, floatIndexList);
            foreach (var address in memoryAddresses)
            {
                memory[address] = value;
            }
        }

        private static IEnumerable<ulong> GenerateMemoryPositions(ulong memoryAddressWithoutFloating, List<int> floatingIndexList, int index = 0)
        {
            if (index >= floatingIndexList.Count())
            {
                yield return memoryAddressWithoutFloating;
            }
            else
            {
                int maskIndex = floatingIndexList[index];
                ulong mask = (ulong)Math.Pow(2, maskIndex);
                foreach (var x in GenerateMemoryPositions(memoryAddressWithoutFloating | mask, floatingIndexList, index + 1))
                {
                    yield return x;
                }
                foreach (var x in GenerateMemoryPositions(memoryAddressWithoutFloating & ~mask, floatingIndexList, index + 1))
                {
                    yield return x;
                }
            }
        }

        private static void ProcessInstruction2((string, string) instruction, Dictionary<ulong, ulong> memory,
            ref (ulong zeroMask, IEnumerable<int> floatingIndexList) mask)
        {
            switch (instruction.Item1)
            {
                case "mask":
                    mask = ProcessMemoryMask(instruction.Item2);
                    break;
                default:
                    WriteToMemory2(memory, ulong.Parse(instruction.Item1), ulong.Parse(instruction.Item2), mask);
                    break;
            }
        }

        private static (ulong zeroMask, IEnumerable<int> floatingIndexList) ProcessMemoryMask(string mask)
        {
            string orMaskstring = mask.Replace('X', '0');
            ulong orMask = Convert.ToUInt64(orMaskstring, 2);
            var floatingIndexList = mask.Select((e, i) => (e, mask.Length - i - 1))
                .Where(t => t.e.Equals('X'))
                .Select(t => t.Item2);
            return (orMask, floatingIndexList);
        }

        private static void ProcessInstruction1((string, string) instruction, Dictionary<ulong, ulong> memory,
            ref (ulong zeroMask, ulong oneMask) mask)
        {
            switch (instruction.Item1)
            {
                case "mask":
                    mask = ProcessMask(instruction.Item2);
                    break;
                default:
                    var memoryAddress = ulong.Parse(instruction.Item1);
                    memory[memoryAddress] = ApplyMask(ulong.Parse(instruction.Item2), mask);
                    break;
            }
        }

        private static ulong ApplyMask(ulong value, (ulong zeroMask, ulong oneMask) mask)
        {
            ulong result = value & mask.zeroMask;
            result |= mask.oneMask;
            return result;
        }

        private static (ulong zeroMask, ulong oneMask) ProcessMask(string mask)
        {
            string zeroMaskString = mask.Replace('X', '1');
            ulong zeroMask = Convert.ToUInt64(zeroMaskString, 2);
            string oneMaskString = mask.Replace('X', '0');
            ulong oneMask = Convert.ToUInt64(oneMaskString, 2);
            return (zeroMask, oneMask);
        }

        private static (string, string) ParseInstruction(string e)
        {
            var tokens = e.Split(new string[] { " = ", "mem[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            return (tokens[0], tokens[1]);
        }
    }
}
