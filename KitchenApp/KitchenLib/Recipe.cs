using System;

namespace KitchenLib
{
    public class Recipe
    {
        public string _title { get; set; }
        public string _image { get; set; }
        public uint _id { get; set; }
        public List<Product> _ingridients{ get; set; }
        public List<string> _preparation{ get; set; }

        public Recipe(string title, string image, uint id, List<Product> ingridients, List<string> preparation)
        {
            _title = title;
            _image = image;
            _id = id;
            _ingridients = ingridients;
            _preparation = preparation;
        }

        public Recipe()
        {
        }
    }
}
