using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class WishlistStore
    {
        public string User { get; set; }

        public WishlistStore(string user)
        {
            User = user;
        }

        public async Task<bool> Exists(string uid)
        {
            Boolean exists;
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[:WSH]->(i:Wishlist) WHERE u._email = $email AND i.name = $name RETURN i.name",
                        new {email = User, name = uid});
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

        public async Task Add(string name)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("MATCH (u:User) where u._name = $user " +
                                      "CREATE (i:Wishlist {name: $name}) " +
                                      "CREATE (u)-[:WSH]->(i)",
                        new {user = User, name});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<bool> Remove(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User)-[:WSH]->(i:Wishlist)" +
                                                   "WHERE u._email = $email AND i._name = $name " +
                                                   "detach delete i",
                        new {email = User, name = uid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public async Task<Wishlist> Get(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
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
                        new {email = User, name = uid});
                    var inv = new Wishlist();
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

        public async void Add_prod(string uid, string prodName)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:WSH]->(i:Wishlist), (p:Product) " +
                                              "where u._email = $email and i.name = $name and p.name = $p_name " +
                                              "create (i)-[:CONTAIN]->(p)");
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<string>> GetAll()
        {
            var l = new List<string>();
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)-[:WSH]->(i:Wishlist) " +
                                              "where u._email = $email " +
                                              "return i._name");
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