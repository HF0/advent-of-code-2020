using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode16
{
    class Program
    {
        static void Main(string[] args)
        {
            var sections = File.ReadAllText("input.txt")
                .Split("\n\n")
                .Select(e => e.Split("\n", StringSplitOptions.RemoveEmptyEntries))
                .ToList();
            var rules = sections[0].Select(e => ParseRule(e)).ToDictionary(k => k.Item1, k => k.Item2);
            var myTicket = sections[1].Skip(1).Select(e => ParseTicket(e)).First().ToList();
            var nearbyTickets = sections[2].Skip(1).Select(e => ParseTicket(e));
            var validRanges = rules.Values.SelectMany(e => e);

            var solution1 = nearbyTickets
                .SelectMany(ticket => InvalidFields(ticket, validRanges))
                .Sum();
            Console.WriteLine($"Solution 1: {solution1}");

            var possibleFields = GetPossibleFields(rules, nearbyTickets, validRanges)
                .Select((e,i) => (i, e))
                .OrderBy(t => t.e.Count())
                .ToList();

            var solution = FindFieldAssignment(0, new List<string>(), possibleFields, myTicket.Count());
            long solution2 = solution
                .Select((rule, index) => (rule, index))
                .Where(t => t.rule.StartsWith("departure"))
                .Select(t => myTicket[t.index])
                .Aggregate(1L, (a, b) => a * b);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static List<HashSet<string>> GetPossibleFields(Dictionary<string, IEnumerable<(int, int)>> rules, IEnumerable<IEnumerable<int>> nearbyTickets, IEnumerable<(int, int)> validRanges)
        {
            var validTickets = nearbyTickets
                .Where(ticket => InvalidFields(ticket, validRanges).Count() == 0)
                .Select(e => e.ToList());
            List<HashSet<string>> validRulesPerfield = Enumerable
                .Range(0, validTickets.First().Count).Select(_ => rules.Keys.ToHashSet())
                .ToList();
            foreach (var ticket in validTickets)
            {
                for (int i = 0; i < ticket.Count(); i++)
                {
                    HashSet<string> fieldRules = validRulesPerfield[i];
                    int rulesBefore = fieldRules.Count();
                    var field = ticket[i];
                    fieldRules.RemoveWhere(ruleName => !ValidRange(field, rules[ruleName]));
                    if (fieldRules.Count() == 1 && rulesBefore > 1)
                    {
                        foreach (var remainingRules in validRulesPerfield.Where(e => e.Count() > 1))
                        {
                            remainingRules.Remove(fieldRules.First());
                        }
                    }
                }
            }
            return validRulesPerfield;
        }

        private static IEnumerable<string> FindFieldAssignment(int position, List<string> solution,
            List<(int,HashSet<string>)> validRulesPerPosition, int numFields)
        {
            if (solution.Count() == numFields)
            {
                return validRulesPerPosition
                    .Select(e => e.Item1)
                    .Zip(solution)
                    .OrderBy(e => e.First)
                    .Select(e => e.Second);
            }
            IEnumerable<string> validFields = validRulesPerPosition[position].Item2.Where(r => !solution.Contains(r));
            if (validFields.Count() == 0)
            {
                return null;
            }
            foreach (var possibleField in validFields)
            {
                solution.Add(possibleField);
                var result = FindFieldAssignment(position + 1, solution, validRulesPerPosition, numFields);
                if (result != null)
                {
                    return result;
                }
                solution.Remove(possibleField);
            }
            return null;
        }

        private static IEnumerable<int> InvalidFields(IEnumerable<int> ticket, IEnumerable<(int, int)> validRanges)
        {
            return ticket.Where(e => !ValidRange(e, validRanges));
        }

        private static bool ValidRange(int fieldValue, IEnumerable<(int, int)> validRanges)
        {
            return validRanges.Any(range => fieldValue >= range.Item1 && fieldValue <= range.Item2);
        }

        private static IEnumerable<int> ParseTicket(string ticketString)
        {
            return ticketString.Split(',').Select(e => int.Parse(e));
        }

        private static (string, IEnumerable<(int, int)>) ParseRule(string e)
        {
            var tokens = e.Split(": ");
            var ranges = tokens[1].Split(" or ");
            var intRanges = ranges.Select(e => ParseRange(e));
            return (tokens[0], intRanges);
        }

        private static (int, int) ParseRange(string v)
        {
            var tokens = v.Split('-');
            return (int.Parse(tokens[0]), int.Parse(tokens[1]));
        }
    }
}
