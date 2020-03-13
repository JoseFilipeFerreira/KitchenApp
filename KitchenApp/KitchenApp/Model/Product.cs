using System;

namespace KitchenApp.Model
{
    public class Product
    {
        private string _name { get; set; }
        private string _type { get; set; }
        private DateTime _consume_before { get; set; }
        private int _quantity { get; set; }
        private string _owner_uid { get; set; }

        public Product(string name, string type, DateTime consume_before, int quantity, string owner_uid)
        {
            _name = name;
            _type = type;
            _consume_before = consume_before;
            _quantity = quantity;
            _owner_uid = owner_uid;
        }

        public int restock(int quantity)
        {
            _quantity += quantity;
            return _quantity;
        }
    }
}