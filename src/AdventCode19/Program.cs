using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode19
{
    class Program
    {
        static void Main(string[] args)
        {
            var sections = File.ReadAllText("input.txt")
                .Split(Environment.NewLine + Environment.NewLine);

            var messages = sections[1]
                .Split(Environment.NewLine);

            var rules = sections[0]
                .Split(Environment.NewLine)
                .Select(e => ParseRule(e))
                .ToDictionary(k => k.Item1, k => k.Item2);

            int solution1 = messages.Count(m => ValidRule(m, rules[0], rules));
            Console.WriteLine($"Solution 1: {solution1}");

            var r1 = ParseRule("8: 42 | 42 8");
            var r2 = ParseRule("11: 42 31 | 42 11 31");
            rules[r1.Item1] = r1.Item2;
            rules[r2.Item1] = r2.Item2;
            int solution2 = messages.Count(m => ValidRule(m, rules[0], rules));
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static bool ValidRule(string message, IEnumerable<string> listRule,
            Dictionary<int, IEnumerable<IEnumerable<string>>> rules)
        {
            if (message.Length == 0 && listRule.Count() == 0)
            {
                return true;
            }
            else if (message.Length == 0 || listRule.Count() == 0)
            {
                return false;
            }
            bool isValid;
            var firstRule = listRule.First();
            var otherRules = listRule.Skip(1);
            if (IsSimpleRule(firstRule))
            {
                string simpleRuleString = GetSimpleRuleValue(firstRule);
                isValid = message.StartsWith(simpleRuleString) && 
                    ValidRule(message[simpleRuleString.Length..], otherRules, rules);
            }
            else
            {
                int ruleNumber = int.Parse(firstRule);
                isValid = rules[ruleNumber]
                    .Select(subRule => subRule.Concat(otherRules))
                    .Any(rule => ValidRule(message, rule, rules));
            }
            return isValid;
        }
        private static bool ValidRule(string message, IEnumerable<IEnumerable<string>> conditions,
            Dictionary<int, IEnumerable<IEnumerable<string>>> rules) => conditions.Any(r => ValidRule(message, r, rules));

        private static string GetSimpleRuleValue(string first) => first.Replace("\"", "");

        private static bool IsSimpleRule(string ruleCondition) => ruleCondition.Contains("\"");

        private static (int, IEnumerable<IEnumerable<string>>) ParseRule(string ruleString)
        {
            var tokens = ruleString.Split(": ");
            return (int.Parse(tokens[0]),
                tokens[1].Split("|").Select(e => e.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
