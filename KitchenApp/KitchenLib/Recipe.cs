using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace KitchenLib
{
    public class RecipeProduct
    {
        public string id { get; set; }
        public string name { get; set; }
        public uint ammount { get; set; }
        public string unit { get; set; }

        public RecipeProduct(string Id, string Name, uint Ammount, string Unit)
        {
            id = Id;
            name = Name;
            ammount = Ammount;
            unit = Unit;
        }


    }
    public class Recipe
    {
        public string title { get; set; }
        public string image { get; set; }
        public uint id { get; set; }
        public List<RecipeProduct> Ingridients{ get; set; }
        public string Preparation{ get; set; }

        public Recipe(string recipeTitle, string imageURL, uint recipeId, List<RecipeProduct> ingridients, string preparation)
        {
            title = recipeTitle;
            image = imageURL;
            id = recipeId;
            Ingridients = ingridients;
            Preparation = preparation;
        }

        public Recipe()
        {
        }
    }
}
