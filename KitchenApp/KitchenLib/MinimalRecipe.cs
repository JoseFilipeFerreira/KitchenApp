using System;

namespace KitchenLib
{
    public class MinimalRecipe
    {
        public string title { get; set; }
        public string image { get; set; }
        public long id { get; set; }

        public MinimalRecipe(Recipe recipe)
        {
            title = recipe.title;
            image = recipe.image;
            id = recipe.id;
        }

        public MinimalRecipe()
        {
        }

        public Recipe ToRecipe(string API_KEY)
        {
            return RecipeSearch.SearchSingleRecipe(API_KEY, this);
        }
    }
}