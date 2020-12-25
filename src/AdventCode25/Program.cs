using System;
using System.IO;
using System.Linq;

namespace AdventCode25
{
    class Program
    {
        private const long InitialSubjectNumber = 7;
        private const long DivValue = 20201227L;

        static void Main(string[] args)
        {
            var publicKeys = File.ReadAllLines("input.txt")
                .Select(e => long.Parse(e))
                .ToList();

            var key1 = publicKeys[1];
            var key2 = publicKeys[0];

            long key1LoopSize = FindLoopSize(key1);
            var encryptionKeyA = FindEncryptionKey(key2, key1LoopSize);
            long solution1A = encryptionKeyA;

            long key2LoopSize = FindLoopSize(key2);
            var encryptionKeyB = FindEncryptionKey(key1, key2LoopSize);
            long solution1B = encryptionKeyB;

            Console.WriteLine($"Solution 1: {solution1A} {solution1B}");

        }

        private static long FindLoopSize(long publicKey)
        {
            long value = 1;
            long loopSize = 0;
            do
            {
                value *= InitialSubjectNumber;
                value %= DivValue;
                loopSize++;
            } while (!value.Equals(publicKey));
            return loopSize;
        }

        private static long FindEncryptionKey(long subjetNumber, long loopSize)
        {
            long value = 1;
            for (long i = 0; i < loopSize; i++)
            {
                value *= subjetNumber;
                value %= DivValue;
            }
            return value;
        }
    }
}
