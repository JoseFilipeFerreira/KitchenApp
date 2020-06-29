using System.Collections.Generic;

namespace KitchenLib
{
    public class RecipeProduct
    {
        public string id { get; set; }
        public string name { get; set; }
        public string original{ get; set; }
    }
    
    public class Recipe
    {
        public string title { get; set; }
        public string image { get; set; }
        public long id { get; set; }
        public List<RecipeProduct> extendedIngredients{ get; set; }
        public string summary{ get; set; }
        public string instructions{ get; set; }
    }
}
