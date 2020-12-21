using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Report.Start();
            var foods = LoadFoos("input.txt");

            var allergen2Ingredient = foods.SelectMany(f => f.allergens.Select(allergen => (allergen, f.ingredients)))
                .GroupBy(k => k.allergen, v => v.ingredients,
                    (allergen, ingredients) => (allergen, ingredients: ingredients.IntersectMany().ToList()))
                .ToDictionary(k => k.allergen, v => v.ingredients);

            while (allergen2Ingredient.Any(kvp => kvp.Value.Count > 1))
            {
                var unique = allergen2Ingredient.Where(kvp => kvp.Value.Count == 1).SelectMany(kvp => kvp.Value.ToList());
                foreach (var ambigious in allergen2Ingredient.Values.Where(l => l.Count > 1))
                {
                    ambigious.RemoveAll(a => unique.Contains(a));
                }
            }

            var allergenicIngredients = allergen2Ingredient.SelectMany(kvp => kvp.Value).ToList();
            var allIngredients = foods.SelectMany(f => f.ingredients).ToList();
            var allergenFreeIngredients = allIngredients.Except(allergenicIngredients).ToHashSet();

            allIngredients
                .Where(ingredient => allergenFreeIngredients.Contains(ingredient))
                .Count()
                .AsResult1();
            allergen2Ingredient
                .OrderBy(kvp => kvp.Key)
                .SelectMany(kvp => kvp.Value)
                .ToCommaString(",")
                .AsResult2();

            Report.End();
        }

        public static List<(List<string> ingredients, List<string> allergens)> LoadFoos(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(s =>
                {
                    var parts = s.Splizz(" (contains ").ToList();
                    var ingredients = parts.First().Splizz(" ").ToList();
                    var allergens = parts.Last().Splizz(", ", ")").ToList();
                    return (ingredients, allergens);
                })
             .ToList();
        }
    }
}
