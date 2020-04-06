using System.Collections.Generic;

namespace KitchenLib
{
    public class Inventory
    {
        public string _name { get; set; }
        public string _owner_id { get; set; }
        public List<string> _guests { get; set; }
        public List<Product> _products { get; set; }
    }
}