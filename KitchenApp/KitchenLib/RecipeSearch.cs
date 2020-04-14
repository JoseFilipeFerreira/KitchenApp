using System;
//Request library
using System.Net;
using System.IO;

namespace KitchenLib
{
    public class RecipeSearch
    {
        public List<Product> _ingridients{ get; set; }

        protected string api = "https://api.spoonacular.com/recipes";
        protected string endpoint = "/findByIngredients&ingridients";

        public RecipeSearch(List<Product> ingridients)
        {
            _ingridients = ingridients;
        }

        public RecipeSearch()
        {
        }

        protected string get(string options){
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api + endpoint + options);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public List<Recipe> GetRecipies(uint number)
        {
            var call = "&ingredients="

            List<string> ingridients_string;
            for(var p in _ingridients)
                ingridients_string.Append(p._name);

            call += string.Join(",", ingridients_string);
            call += "&number="
            call += number;

            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(uri);
            return await Task.Run(() => JsonObject.Parse(content));


        }
    }
}
