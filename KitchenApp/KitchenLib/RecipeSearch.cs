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
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, List<Product> ingridients)
        {
            var options = "number=" + number;

            options += "&includeIngredients=";

            var ingridientsString = ingridients.Select(s => s._name).ToList();
            options += string.Join(",", ingridientsString);
            
            return GetMinimalRecipies(options);
        }
        
        // 2 point 1st recipe + 0.51 point per recipe
        public static List<Recipe> SearchRecipe(uint number, List<Product> ingridients)
        {
            return SearchRecipe(SearchMinimalRecipe(number, ingridients));
        }
        
        // 1 point base + 0.01 points per recipe
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, string recipeName)
        {
            var options = "number=" + number;
            options += "&query=" + recipeName;

            return GetMinimalRecipies(options);
        }
        
        // 2 point 1st recipe + 0.51 point per recipe
        public static List<Recipe> SearchRecipe(uint number, string recipeName)
        {
            return SearchRecipe(SearchMinimalRecipe(number, recipeName));
        }
        
        // 1 point base + 0.01 points per recipe
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, string recipeName, List<Product> ingridients)
        {
            var options = "number=" + number;
            
            options += "&query=" + recipeName;
            
            options += "&includeIngredients=";

            var ingridientsString = ingridients.Select(s => s._name).ToList();
            options += string.Join(",", ingridientsString);
            
            return GetMinimalRecipies(options);
        }
        
        // 2 point 1st recipe + 0.51 point per recipe
        public static List<Recipe> SearchRecipe(uint number, string recipeName, List<Product> ingridients)
        {
            return SearchRecipe(SearchMinimalRecipe(number, recipeName, ingridients));
        }
        
        // 1 point
        public static Recipe SearchSingleRecipe(MinimalRecipe mR)
        {
            var n = new List<MinimalRecipe> {mR};
            return SearchRecipe(n)[0];
        }
        
        // 1 point 1st recipe + 0.5 point per recipe
        public static List<Recipe> SearchRecipe(List<long> minimalListID)
        {
            if (!minimalListID.Any())
            {
                return new List<Recipe>();
            }
            var API_KEY = "7a98067ae9ea425ca548d96347913e74";
            var url = "https://api.spoonacular.com/recipes/informationBulk?ids=";
            
            url += string.Join(",", minimalListID);
            url += "&apiKey=" + API_KEY;

            return JsonConvert.DeserializeObject<List<Recipe>>(get_request(url));
        }
        
        // 1 point 1st recipe + 0.5 point per recipe
        public static List<Recipe> SearchRecipe(List<MinimalRecipe> minimalList)
        {
            return SearchRecipe(minimalList.Select(s => (long)s.id).ToList());
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
        
        // 1 point base + 0.01 points per recipe
        private static List<MinimalRecipe> GetMinimalRecipies(string options)
        {
            var API_KEY = "7a98067ae9ea425ca548d96347913e74";
            var url = "https://api.spoonacular.com/recipes/complexSearch?" + options + "&apiKey=" + API_KEY;
            return JsonConvert.DeserializeObject<RootMinimalRecipes>(get_request(url)).results;
        }
    }
}
