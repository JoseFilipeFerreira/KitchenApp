using System;
using System.Collections.Generic;

namespace KitchenLib
{
    public class Inventory<T> where T : Product
    {
        public string _guid { get; set; }
        public string _name { get; set; }
        public string _owner_id { get; set; }
        public List<string> _guests { get; set; }
        public List<T> _products { get; set; }

        public Inventory(string name)
        {
            _name = name;
            _guid = Guid.NewGuid().ToString();
        }

        public Inventory()
        {
        }
    }
}