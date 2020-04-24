using System;
using System.Collections.Generic;
//Request library
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KitchenLib
{
    class MinimalRecipe
    {
        public string title { get; set; }
        public string image { get; set; }
        public uint id { get; set; }
        
        public MinimalRecipe(string recipeTitle, string imageURL, uint recipeId)
        {
            title = recipeTitle;
            image = imageURL;
            id = recipeId;
        }
    }

    public class RecipeSearch
    {
        private List<Product> Ingridients;
        private string API_KEY = "7a98067ae9ea425ca548d96347913e74";

        private string apiRecepiesIngridients = "https://api.spoonacular.com/recipes/";

        public RecipeSearch(List<Product> ingridients)
        {
            Ingridients = ingridients;
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

        public List<Recipe> GetRecipies(uint number)
        {
            var call = "findByIngredients?ingredients=";

            var ingridientsString = new List<string>();
            foreach (var p in Ingridients)
                ingridientsString.Append(p._name);

            call += string.Join(",", ingridientsString);
            call += "&number=";
            call += number;

            call += "&apiKey=";
            call += API_KEY;
            
            var minimalList = JsonConvert.DeserializeObject<List<MinimalRecipe>>(get_request(apiRecepiesIngridients + call));

            var recipeList = new List<Recipe>();
            foreach (var mR in minimalList)
            {
                string reply = get_request(apiRecepiesIngridients + mR.id + "/information?includeNutrition=false");

                var parsedObject = JObject.Parse(reply);
                
                var instructions = parsedObject["summary"].ToString();

                var RP = new List<RecipeProduct>();
                foreach (var PP in parsedObject["extendedIngredients"])
                {
                    RP.Append(new RecipeProduct(PP["id"].ToString(), PP["name"].ToString(), (uint) PP["measures"]["metric"]["amount"],
                        PP["measures"]["metric"]["unitShort"].ToString()));
                }


                recipeList.Append(new Recipe(mR.title, mR.image, mR.id, RP, instructions));
            }


            return recipeList;




        }
    }
}
