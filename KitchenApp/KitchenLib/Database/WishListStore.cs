using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class WishlistStore
    {
        public static async Task<bool> Exists(string uid, string user)
        {
            Boolean exists;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[:WSH]->(i:Wishlist) WHERE u._email = $email AND i.name = $name RETURN i.name",
                        new {email = user, name = uid});
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

        public static async Task Add(string name, string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("MATCH (u:User) where u._name = $user " +
                                      "CREATE (i:Wishlist {name: $name}) " +
                                      "CREATE (u)-[:WSH]->(i)",
                        new {user, name});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<bool> Remove(string uid, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User)-[:WSH]->(i:Wishlist)" +
                                                   "WHERE u._email = $email AND i._name = $name " +
                                                   "detach delete i",
                        new {email, name = uid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public static async Task<Inventory<Product>> Get(string uid, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "Match(u:User)-[:WSH]->(i:Wishlist)-[c:CONTAIN]->(p:Product) " +
                        "Match(i)-[:SHARED]->(z:User)" +
                        "Where u._email = $email AND i.name = $name " +
                        "Return [(a)-[c:CONTAIN]->(b) where b: Product] as products, " +
                        "[(a)-[:Shared]->(b) where b: User | b] as guests " +
                        "u._email as owner_id, " +
                        "i.name as name,",
                        new {email, name = uid});
                    var inv = new Inventory<Product>();
                    while (await reader.FetchAsync())
                    {
                        var prods = reader.Current["products"].As<IList<INode>>();
                        var guests = reader.Current["guests"].As<IList<INode>>();
                        inv._name = reader.Current["name"].As<string>();
                        inv._owner_id = reader.Current["owner_id"].As<string>();
                        inv._guests = new List<string>();
                        foreach (var guest in guests)
                        {
                            inv._guests.Append(guest["_email"].As<string>());
                        }

                        foreach (var prod in prods)
                        {
                            var u = new Product();
                            foreach (var (key, value) in prod.Properties)
                            {
                                u.GetType().GetProperty(key)?.SetValue(u, value, null);
                            }

                            inv._products.Append(u);
                        }
                    }

                    return inv;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return null;
        }

        public static async void Add_prod(string uid, string prodName, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:WSH]->(i:Wishlist), (p:Product) " +
                                              "where u._email = $email and i.name = $uid and p.name = $prodName " +
                                              "create (i)-[:CONTAIN]->(p)", new {email, uid, prodName});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<List<string>> GetAll(string email)
        {
            var l = new List<string>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)-[:WSH]->(i:Wishlist) " +
                                              "where u._email = $email " +
                                              "return i._name", new {email});
                    while (await r.FetchAsync())
                    {
                        l.Add(r.Current[0].As<string>());
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