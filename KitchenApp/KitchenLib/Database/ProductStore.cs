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

        public static async Task<List<Product>> Search(string regex)
        {
            var l = new List<Product>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(p:Product) where p._name =~ '$regex' return p", new {regex});
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
    }
}