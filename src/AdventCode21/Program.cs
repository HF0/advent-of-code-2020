using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode21
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputData = File.ReadAllLines("input.txt")
                .Select(e => ParseIngredients(e));

            var filteredAllergenList = inputData
                .SelectMany(e => ToAllergenGroup(e))
                .GroupBy(k => k.allergen, e => e.ingredients)
                .Select(e => (e.Key, e.Aggregate((a, b) => a.Intersect(b).ToHashSet())))
                .ToDictionary(k => k.Key, k => k.Item2);

            var ingredientGroups = inputData
                .Select(e => e.ingredients)
                .ToList();

            var solution = GetIngredientsWithoutAllergens(filteredAllergenList, ingredientGroups);

            int solution1 = ingredientGroups
                .Select(e => e.Intersect(solution.Item1).Count())
                .Sum();
            Console.WriteLine($"Solution 1: {solution1}");

            string solution2 = string.Join(',', 
                solution.Item2
                .OrderBy(e => e.Value)
                .Select(e => e.Key));
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static (ISet<string>, Dictionary<string, string>) GetIngredientsWithoutAllergens(Dictionary<string, ISet<string>> filteredAllergenList, List<ISet<string>> ingredientGroups)
        {
            ISet<string> remainingIngredients = ingredientGroups
                .Aggregate((a, b) => a.Union(b).ToHashSet());

            HashSet<string> ingredientsToRemove;
            Dictionary<string, string> ingredientAllergenDictionary = new Dictionary<string, string>();
            do
            {
                var clearAllergens = filteredAllergenList
                    .Where(k => k.Value.Count() == 1);
                foreach (var c in clearAllergens)
                {
                    ingredientAllergenDictionary.Add(c.Value.First(), c.Key);
                }
                ingredientsToRemove = clearAllergens
                    .SelectMany(e => e.Value)
                    .ToHashSet();

                foreach (ISet<string> sets in filteredAllergenList.Values)
                {
                    sets.ExceptWith(ingredientsToRemove);
                }
                foreach (string ingredient in ingredientsToRemove)
                {
                    filteredAllergenList.Remove(ingredient);
                    remainingIngredients.Remove(ingredient);
                }
            } while (ingredientsToRemove.Count() > 0);
            return (remainingIngredients, ingredientAllergenDictionary);
        }

        private static (ISet<string> ingredients, ISet<string> allergens) ParseIngredients(string e)
        {
            var tokens = e.Split(new[] { " (contains ", ")" }, StringSplitOptions.RemoveEmptyEntries);
            var ingredients = tokens[0].Split(' ');
            var allergens = tokens[1].Split(", ");
            return (ingredients.ToHashSet(), allergens.ToHashSet());
        }

        private static IEnumerable<(string allergen, ISet<string> ingredients)> ToAllergenGroup((ISet<string> ingredients, ISet<string> allergens) rule)
        {
            return rule.allergens
                .Select(e => (e, (ISet<string>)new HashSet<string>(rule.ingredients)));
        }
    }
}
