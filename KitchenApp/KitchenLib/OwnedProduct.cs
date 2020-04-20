using System;

namespace KitchenLib
{
    public class OwnedProduct : Product
    { 
        public DateTime _consume_before { get; set; }
        public uint _stock { get; set; }
        public string _owner_uid { get; set; }

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