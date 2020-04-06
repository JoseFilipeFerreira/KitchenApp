using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class InventoryStory
    {
        public string _user { get; set; }

        public InventoryStory(string user)
        {
            _user = user;
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
                        "MATCH(u:User)-[:INV]->(i:Inventory) WHERE u._email = $email AND i.name = $name RETURN i.name",
                        new {email = _user, name = uid});
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
                                      "CREATE (i:Inventory {name: $name}) " +
                                      "CREATE (u)-[:INV]->(i)",
                        new {user = _user, name});
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
                    var reader = await tx.RunAsync("MATCH(u:User)-[:INV]->(i:Inventory)" +
                                                   "WHERE u._email = $email AND i._name = $name " +
                                                   "detach delete i",
                        new {email = _user, name = uid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public async Task<Inventory> Get(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "Match(u:User)-[:INV]->(i:Inventory)-[c:CONTAIN]->(p:Product) " +
                        "Match(i)-[:SHARED]->(z:User)" +
                        "Where u._email = $email AND i.name = $name " +
                        "Return [(a)-[c:CONTAIN]->(b) where b: Product | " +
                        "{ prod: b, quant: c.quantity, expire: c.expiration_date }] as products, " +
                        "[(a)-[:Shared]->(b) where b: User | b] as guests " +
                        "u._email as owner_id, " +
                        "i.name as name,",
                        new {email = _user, name = uid});
                    var inv = new Inventory();
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
                            var u = new Product();
                            foreach (var (key, value) in prod["prod"].As<INode>().Properties)
                            {
                                u.GetType().GetProperty(key)?.SetValue(u, value, null);
                            }

                            u._consume_before = prod["expire"].As<DateTime>();
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

        public async void add_prod(string uid, string prod_name, int quant, DateTime date)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:INV]->(i:Inventory), (p:Product) " +
                                              "where u._email = $email and i.name = $name and p.name = $p_name " +
                                              "create (i)-[:CONTAIN {quantity: $quant, expiration_date: $date");
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async void restock(string uid, string prod_name, int quant)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync(
                        "Match (u:User)-[:INV]->(i:Inventory)-[c:CONTAIN]->(p:Product) " +
                        "where u._email = $email and i.name = $name and p.name = $p_name " +
                        "Set c.quantity = $quant", new {name = uid, p_name = prod_name, email = _user, quant});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}