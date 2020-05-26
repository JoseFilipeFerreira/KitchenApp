using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace KitchenLib
{
    class RootMinimalRecipes
    {
        public List<MinimalRecipe> results { get; set; }
    }

    public class RecipeSearch
    {
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, List<Product> ingridients)
        {
            var options = "number=" + number;

            options += "&includeIngredients=";

            var ingridientsString = ingridients.Select(s => s._name).ToList();
            options += string.Join(",", ingridientsString);
            
            return GetMinimalRecipies(options);
        }
        
        public static List<Recipe> SearchRecipe(uint number, List<Product> ingridients)
        {
            return SearchRecipe(SearchMinimalRecipe(number, ingridients));
        }
        
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, string recipeName)
        {
            var options = "number=" + number;
            options += "&query=" + recipeName;

            return GetMinimalRecipies(options);
        }
        
        public static List<Recipe> SearchRecipe(uint number, string recipeName)
        {
            return SearchRecipe(SearchMinimalRecipe(number, recipeName));
        }
        
        public static List<MinimalRecipe> SearchMinimalRecipe(uint number, string recipeName, List<Product> ingridients)
        {
            var options = "number=" + number;
            
            options += "&query=" + recipeName;
            
            options += "&includeIngredients=";

            var ingridientsString = ingridients.Select(s => s._name).ToList();
            options += string.Join(",", ingridientsString);
            
            return GetMinimalRecipies(options);
        }
        
        public static List<Recipe> SearchRecipe(uint number, string recipeName, List<Product> ingridients)
        {
            return SearchRecipe(SearchMinimalRecipe(number, recipeName, ingridients));
        }
        
        public static Recipe SearchSingleRecipe(MinimalRecipe mR)
        {
            var n = new List<MinimalRecipe>();
            n.Append(mR);
            return SearchRecipe(n)[0];
        }

        public static List<Recipe> SearchRecipe(List<MinimalRecipe> minimalList)
        {
            var API_KEY = "7a98067ae9ea425ca548d96347913e74";
            var url = "https://api.spoonacular.com/recipes/informationBulk?ids=";

            var recipeString = minimalList.Select(s => s.id.ToString()).ToList();
            url += string.Join(",", recipeString);
            url += "&apiKey=" + API_KEY;

            return JsonConvert.DeserializeObject<List<Recipe>>(get_request(url));
        }

        
        
        private static string get_request(string url)
        {
            var httpWebRequestQR = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequestQR.ContentType = "application/json";
            httpWebRequestQR.Method = "GET";

            var httpResponseQr = (HttpWebResponse)httpWebRequestQR.GetResponse();
            using (var streamReader = new StreamReader(httpResponseQr.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
        
        private static List<MinimalRecipe> GetMinimalRecipies(string options)
        {
            var API_KEY = "7a98067ae9ea425ca548d96347913e74";
            string url = "https://api.spoonacular.com/recipes/complexSearch?" + options + "&apiKey=" + API_KEY;
            return JsonConvert.DeserializeObject<RootMinimalRecipes>(get_request(url)).results;
        }
    }
}
