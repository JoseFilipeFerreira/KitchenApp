using System;

namespace KitchenLib
{
    public class Product
    {
        public string _guid { get; set; }
        public string _name { get; set; }
        public string _category { get; set; }
        public long _quantity { get; set; }
        public string _units { get; set; }
        public double _price { get; set; }
        
        public Product() {}

        public Product(string name, string category, long quantity, string units, double price)
        {
            _guid = Guid.NewGuid().ToString();
            _name = name;
            _category = category;
            _quantity = quantity;
            _units = units;
            _price = price;
        }
    }
}