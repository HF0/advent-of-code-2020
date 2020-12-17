using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode13
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var timestamp = long.Parse(lines[0]);
            var buses = GetBuses(lines[1]);
            (long time, int bus) = CalculateSolution1(timestamp, buses);
            long solution1 = (time - timestamp) * bus;
            Console.WriteLine($"Solution 1: {solution1}");

            var restrictions = GetBusesRestrictions(lines[1]);
            long solution2 = CalculateSolution2(restrictions);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static long CalculateSolution2(IEnumerable<(int bus, int minute)> restrictions)
        {
            long increment = restrictions.First().bus;
            long solution = increment;
            foreach ((int bus, int minute) in restrictions.Skip(1))
            {
                // t + i = bus * a     =>    t = bus * a + bus - i
                int reminder = bus - minute % bus;
                while (solution % bus != reminder)
                {
                    solution += increment;
                }
                increment = (long)Lcm(increment, bus);
            }
            return solution;
        }

        private static double Lcm(double a, double b) => a * b / Gcd(a, b);

        private static double Gcd(double a, double b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            double remainder = a % b;
            while (remainder > 0)
            {
                remainder = a % b;
                a = b;
                b = remainder;
            };
            return a;
        }

        private static IEnumerable<(int, int)> GetBusesRestrictions(string v)
        {
            return v.Split(',')
                .Select((v, i) => (v, i))
                .Where(t => !t.v.Equals("x"))
                .Select(t => (int.Parse(t.v), t.i));
        }

        private static (long time, int bus) CalculateSolution1(long timestamp, IEnumerable<int> buses)
        {
            long minTimestamp = long.MaxValue;
            int busMinTimestamp = 0;
            foreach (var bus in buses)
            {
                long time = (long)Math.Ceiling(timestamp / (float)bus) * bus;
                if (time < minTimestamp)
                {
                    minTimestamp = time;
                    busMinTimestamp = bus;
                }
            }
            return (minTimestamp, busMinTimestamp);
        }

        private static IEnumerable<int> GetBuses(string v)
        {
            return v.Split(',')
                .Where(e => !e.Equals("x"))
                .Select(e => int.Parse(e));
        }
    }
}
