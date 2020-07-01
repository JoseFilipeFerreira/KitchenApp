using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class ProductStore
    {
        public static async Task<bool> Exists(string uid)
        {
            bool exists;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:Product) WHERE u._guid = $guid RETURN u._guid",
                        new {guid = uid});
                    while (await reader.FetchAsync())
                        lst.Add(reader.Current[0].ToString());

                    return lst.Count != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return exists;
        }

        public static async Task Add(Product u)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync(
                        "CREATE (:Product {_guid: $uid, _name: $name, _category: $category, _quantity: $quantity, _price: $price, _units = $u._units})",
                        new
                        {
                            name = u._name, category = u._category, quantity = u._quantity, price = u._price,
                            guid = u._guid, units = u._units, uid = u._guid
                        });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<bool> Remove(string guid)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("MATCH(u:Product) WHERE u._guid = $guid detach delete u",
                        new {guid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public static async Task<Product> Edit(string uid,
            string name = null,
            string category = null,
            uint? quantity = null, string units = null, float? price = null)
        {
            Product p = null;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            if (name == null && category == null && quantity == null && units == null && price == null) return null;
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var query = "Match (p:Product) " +
                                "where p._guid = $pguid ";
                    IDictionary<string, object> dic = new Dictionary<string, object>
                        {{"pguid", uid}};
                    if (name != null)
                    {
                        query += "set p._name = $name ";
                        dic.Add("name", name);
                    }

                    if (category != null)
                    {
                        query += "set p._category = $category ";
                        dic.Add("category", category);
                    }

                    if (quantity != null)
                    {
                        query += "set p._quantity = $quantity ";
                        dic.Add("quantity", quantity);
                    }

                    if (units != null)
                    {
                        query += "set p._units = $units ";
                        dic.Add("units", units);
                    }

                    if (price != null)
                    {
                        query += "set p._price = $price ";
                        dic.Add("price", price);
                    }

                    query += "return p";
                    var reader = await tx.RunAsync(query, dic);
                    p = new Product();
                    while (await reader.FetchAsync())
                    {
                        var aa = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in aa)
                        {
                            p.GetType().GetProperty(key)?.SetValue(p, value, null);
                        }
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return p;
        }

        public static async Task<Product> Get(string uid)
        {
            Product u = null;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:Product) Where u._guid = $guid Return u",
                        new {guid = uid});
                    u = new Product();
                    while (await reader.FetchAsync())
                    {
                        var aa = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in aa)
                        {
                            u.GetType().GetProperty(key)?.SetValue(u, value, null);
                        }
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return u;
        }

        public static async Task<List<Product>> GetAll()
        {
            var l = new List<Product>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(p:Product) return p");
                    while (await reader.FetchAsync())
                    {
                        var u = new Product();
                        var curr = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in curr)
                        {
                            u.GetType().GetProperty(key)?.SetValue(u, value, null);
                        }

                        l.Add(u);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }

        public static async Task<List<Product>> GetAll(string category)
        {
            var l = new List<Product>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(p:Product) where p._category = $category return p", new {category});
                    while (await reader.FetchAsync())
                    {
                        var u = new Product();
                        var curr = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in curr)
                        {
                            u.GetType().GetProperty(key)?.SetValue(u, value, null);
                        }

                        l.Add(u);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }
        
        public static async Task<List<Product>> Search(string regex)
        {
            var l = new List<Product>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader =
                        await tx.RunAsync(
                            "Match(p:Product) where p._name =~ '(?i)$regex' or toLower(p._name) starts with toLower($regex) return p",
                            new {regex});
                    while (await reader.FetchAsync())
                    {
                        var u = new Product();
                        var curr = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in curr)
                        {
                            u.GetType().GetProperty(key)?.SetValue(u, value, null);
                        }

                        l.Add(u);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }
        
        public static async Task<List<Product>> Search(string regex, string category)
        {
            var l = new List<Product>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader =
                        await tx.RunAsync(
                            "Match(p:Product) where p._category = $category and p._name =~ '(?i)$regex' or toLower(p._name) starts with toLower($regex) return p",
                            new {regex, category});
                    while (await reader.FetchAsync())
                    {
                        var u = new Product();
                        var curr = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in curr)
                        {
                            u.GetType().GetProperty(key)?.SetValue(u, value, null);
                        }

                        l.Add(u);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }

        public static async Task<List<string>> Categories()
        {
            var l = new List<string>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader =
                        await tx.RunAsync(
                            "Match(p:Product) return distinct p._category");
                    while (await reader.FetchAsync())
                    {
                        var curr = reader.Current[0].As<string>();
                        l.Add(curr);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }
    }
}