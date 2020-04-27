using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class ShoppingListStore
    {
        public string User { get; set; }

        public ShoppingListStore(string user)
        {
            User = user;
        }

        public async Task<bool> Exists(string uid)
        {
            Boolean exists;
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            //var session = new Database("", "", "").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[:SHP]->(i:Shoppinglist) WHERE u._email = $email AND i.name = $name RETURN i.name",
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
                                      "CREATE (i:Shoppinglist {name: $name}) " +
                                      "CREATE (u)-[:SHP]->(i)",
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
                    var reader = await tx.RunAsync("MATCH(u:User)-[:SHP]->(i:Shoppinglist)" +
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

        public async Task<Inventory<WantedProduct>> Get(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "Match(u:User)-[:SHP]->(i:Shoppinglist)-[c:CONTAIN]->(p:Product) " +
                        "Match(i)-[:SHARED]->(z:User)" +
                        "Where u._email = $email AND i.name = $name " +
                        "Return [(a)-[c:CONTAIN]->(b) where b: Product | " +
                        "{ prod: b, quant: c.quantity }] as products, " +
                        "[(a)-[:Shared]->(b) where b: User | b] as guests " +
                        "u._email as owner_id, " +
                        "i.name as name,",
                        new {email = User, name = uid});
                    var inv = new Inventory<WantedProduct>();
                    while (await reader.FetchAsync())
                    {
                        var prods = reader.Current["products"].As<IList<IDictionary<string, object>>>();
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
                            var u = new WantedProduct();
                            foreach (var (key, value) in prod["prod"].As<INode>().Properties)
                            {
                                u.GetType().GetProperty(key)?.SetValue(u, value, null);
                            }

                            u._stock = prod["quant"].As<uint>();

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

        public async void Add_prod(string uid, string prodName, int quant, DateTime date)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:SHP]->(i:Shoppinglist), (p:Product) " +
                                              "where u._email = $email and i.name = $name and p.name = $p_name " +
                                              "create (i)-[:CONTAIN {quantity: $quant, expiration_date: $date}]->(p)",
                        new {date, quant, email = User, name = uid, p_name = prodName});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async void Restock(string uid, string prodName, int quant)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync(
                        "Match (u:User)-[:SHP]->(i:Shoppinglist)-[c:CONTAIN]->(p:Product) " +
                        "where u._email = $email and i.name = $name and p.name = $p_name " +
                        "Set c.quantity = $quant", new {name = uid, p_name = prodName, email = User, quant});
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
                    var r = await tx.RunAsync("match (u:User)-[:SHP]->(i:Shoppinglist) " +
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