using System;

namespace KitchenLib
{
    public class OwnedProduct : Product
    { 
        public DateTime _consume_before { get; set; }
        public uint _stock { get; set; }
        public string _owner_uid { get; set; }

        public OwnedProduct(string name, string category, DateTime consume_before, uint quantity, string units, string owner_uid, float price)
        {
            _name = name;
            _category = category;
            _consume_before = consume_before;
            _quantity = quantity;
            _owner_uid = owner_uid;
            _price = price;
            _guid = new Guid().ToString();
            _units = units;
        }

        public OwnedProduct()
        {
        }

        public uint restock(uint quantity)
        {
            _quantity += quantity;
            return _quantity;
        }
    }
}