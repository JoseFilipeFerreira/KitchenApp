using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace KitchenLib
{
    internal class RootMinimalRecipes
    {
        public List<MinimalRecipe> results { get; set; }
    }

    public class RecipeSearch
    {
        // 1 point base + 0.01 points per recipe
        public static List<MinimalRecipe> SearchMinimalRecipe(string API_KEY, uint number, List<Product> ingredients)
        {
            var options = "number=" + number;

            var ingredientsString = ingredients.Select(s => s._name).ToList();
            options += "&ingredients=";
            options += string.Join(",", ingredientsString);
            
            return GetMinimalIngredients(API_KEY, options);
        }
        
        // 2 point 1st recipe + 0.51 point per recipe
        public static List<Recipe> SearchRecipe(string API_KEY, uint number, List<Product> ingredients)
        {
            return SearchRecipe(API_KEY, SearchMinimalRecipe(API_KEY, number, ingredients));
        }
        
        // 1 point base + 0.01 points per recipe
        public static List<MinimalRecipe> SearchMinimalRecipe(string API_KEY, uint number, string recipeName)
        {
            var options = "number=" + number;
            options += "&query=" + recipeName;

            return GetMinimalRecipies(API_KEY, options);
        }
        
        // 2 point 1st recipe + 0.51 point per recipe
        public static List<Recipe> SearchRecipe(string API_KEY, uint number, string recipeName)
        {
            return SearchRecipe(API_KEY, SearchMinimalRecipe(API_KEY, number, recipeName));
        }

        // 1 point
        public static Recipe SearchSingleRecipe(string API_KEY, MinimalRecipe mR)
        {
            var n = new List<MinimalRecipe> {mR};
            return SearchRecipe(API_KEY, n)[0];
        }
        
        // 1 point 1st recipe + 0.5 point per recipe
        public static List<Recipe> SearchRecipe(string API_KEY, List<long> minimalListID)
        {
            if (!minimalListID.Any())
            {
                return new List<Recipe>();
            }
            var url = "https://api.spoonacular.com/recipes/informationBulk?ids=";
            
            url += string.Join(",", minimalListID);
            url += "&apiKey=" + API_KEY;

            return JsonConvert.DeserializeObject<List<Recipe>>(get_request(url));
        }
        
        // 1 point 1st recipe + 0.5 point per recipe
        public static List<Recipe> SearchRecipe(string API_KEY, List<MinimalRecipe> minimalList)
        {
            return SearchRecipe(API_KEY, minimalList.Select(s => (long)s.id).ToList());
        }
        
        
        // 1 point base + 0.01 points per recipe
        private static List<MinimalRecipe> GetMinimalRecipies(string API_KEY, string options)
        {
            var url = "https://api.spoonacular.com/recipes/complexSearch?"
                      + options
                      + "&instructionsRequired=true&apiKey="
                      + API_KEY;
            return JsonConvert.DeserializeObject<RootMinimalRecipes>(get_request(url)).results;
        }
        
        // 1 point base + 0.01 points per recipe
        private static List<MinimalRecipe> GetMinimalIngredients(string API_KEY, string options)
        {
            var url = "https://api.spoonacular.com/recipes/findByIngredients?"
                      + options
                      + "&instructionsRequired=true&apiKey="
                      + API_KEY;
            return JsonConvert.DeserializeObject<RootMinimalRecipes>(get_request(url)).results;
        }

        private static string get_request(string url)
        {
            var httpWebRequestQr = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequestQr.ContentType = "application/json";
            httpWebRequestQr.Method = "GET";
            Console.WriteLine(url);

            var httpResponseQr = (HttpWebResponse)httpWebRequestQr.GetResponse();
            using var streamReader = new StreamReader(httpResponseQr.GetResponseStream());
            return streamReader.ReadToEnd();
        }
    }
}
