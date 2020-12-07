using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCode4
{
    class Program
    {
        private static Func<string, bool> AlwaysValidRule = x => true;

        private static readonly Dictionary<string, Func<string, bool>> ValidationRules = new Dictionary<string, Func<string, bool>> {
            {"byr", AlwaysValidRule},
            {"iyr", AlwaysValidRule},
            {"eyr", AlwaysValidRule},
            {"hgt", AlwaysValidRule},
            {"hcl", AlwaysValidRule},
            {"ecl", AlwaysValidRule},
            {"pid", AlwaysValidRule},
        };

        private static readonly Dictionary<string, Func<string, bool>> ValidationRules2 = new Dictionary<string, Func<string, bool>> {
            {"byr", v => IsNumberBetween(v, 1920, 2002)},
            {"iyr", v => IsNumberBetween(v, 2010, 2020)},
            {"eyr", v => IsNumberBetween(v, 2020, 2030)},
            {"hgt", v => ValidHeight(v)},
            {"hcl", v => Regex.IsMatch(v, "^#[0-9a-f]{6}$")},
            {"ecl", v => (new [] {"amb","blu","brn","gry","grn","hzl","oth"}).Contains(v) },
            {"pid", v => Regex.IsMatch(v, "^[0-9]{9}$")}
        };

        private static void Main(string[] args)
        {
            Advent4();
        }

        private static void Advent4()
        {
            Console.WriteLine("Solution 1: " + CheckValidCredentials(ValidationRules));
            Console.WriteLine("Solution 2: " + CheckValidCredentials(ValidationRules2));
        }

        private static int CheckValidCredentials(Dictionary<string, Func<string, bool>> validationRules)
        {
            return File.ReadAllText("input.txt")
                .Split("\n\n")
                .Select(e => ParseCredentials(e))
                .Where(e => ValidateCredentials(e, validationRules))
                .Count();
        }

        private static Dictionary<string, string> ParseCredentials(string credentialString)
        {
            return credentialString
                .Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Split(":"))
                .ToDictionary(e => e[0], e => e[1]);
        }

        private static bool IsNumberBetween(string value, int minInclude, int MaxInclude)
        {
            bool number = int.TryParse(value, out int digit);
            return number && digit >= minInclude && digit <= MaxInclude;
        }

        private static bool ValidHeight(string v)
        {
            string unit = v[^2..];
            string stringNumber = v[0..^2];
            bool result;
            switch (unit)
            {
                case "cm":
                    result = IsNumberBetween(stringNumber, 150, 193);
                    break;
                case "in":
                    result = IsNumberBetween(stringNumber, 59, 76);
                    break;
                default:
                    return false;
            };
            return result;
        }

        private static bool ValidateCredentials(Dictionary<string, string> credentials, Dictionary<string, Func<string, bool>> validationDict)
        {
            credentials.Remove("cid");
            if (credentials.Count() != validationDict.Count())
            {
                return false;
            }
            foreach (var fieldValidation in validationDict)
            {
                string field = fieldValidation.Key;
                if (!credentials.ContainsKey(field))
                {
                    return false;
                }
                string value = credentials[field];
                var validateFunction = fieldValidation.Value;
                if (!validateFunction(value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
