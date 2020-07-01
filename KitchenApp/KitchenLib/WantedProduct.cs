using System;

namespace KitchenLib
{
    public class WantedProduct : Product
    { 
        public long _stock { get; set; }
        public string _owner_uid { get; set; }

        public WantedProduct(string name, string category, long quantity, string units, string owner_uid, double price, long stock)
        {
            _name = name;
            _category = category;
            _quantity = quantity;
            _owner_uid = owner_uid;
            _price = price;
            _guid = new Guid().ToString();
            _units = units;
            _stock = stock;
        }

        public WantedProduct()
        {
        }
    }
}