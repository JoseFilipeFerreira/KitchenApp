using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class InventoryStore
    {
        public static async Task<bool> Exists(string name, string email)
        {
            bool exists;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[]->(i:Inventory) WHERE u._email = $email AND i.guid = $name RETURN i.name",
                        new {email, name});
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

        public static async Task<bool> ExistName(string name, string email)
        {
            bool exists;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[:INV]->(i:Inventory) WHERE u._email = $email AND i.name = $name RETURN i.name",
                        new {email, name});
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

        public static async Task Add(Inventory<OwnedProduct> inv, string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("MATCH (u:User) where u._email = $user " +
                                      "CREATE (i:Inventory {name: $name, guid: $guid}) " +
                                      "CREATE (u)-[:INV]->(i)",
                        new {user, name = inv._name, guid = inv._guid});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<bool> Remove(string guid, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("MATCH(u:User)-[:INV]->(i:Inventory) " +
                                                   "WHERE u._email = $email AND i.guid = $guid " +
                                                   "detach delete i",
                        new {email, guid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public static async Task<Inventory<OwnedProduct>> Get(string uid, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            Inventory<OwnedProduct> inv = null;
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "Match(u:User)-[]->(i:Inventory) " +
                        "Where u._email = $email AND i.guid = $name " +
                        "Optional match (i)-[c:CONTAIN]->(p:Product) " +
                        "Optional Match(i)-[:SHARED]->(z:User) " +
                        "Return [(i)-[c]->(p) | { prod: p, quant: c.quantity, expire: c.expiration_date }] as products, " +
                        "[(a)-[:Shared]->(b) where b: User | b] as guests, " +
                        "u._email as owner_id, " +
                        "i.name as name, i.guid as guid",
                        new {email, name = uid});
                    while (await reader.FetchAsync())
                    {
                        inv = new Inventory<OwnedProduct>();
                        var prods = reader.Current["products"].As<IList<IDictionary<string, object>>>();
                        var guests = reader.Current["guests"].As<IList<INode>>();
                        inv._name = reader.Current["name"].As<string>();
                        inv._guid = reader.Current["guid"].As<string>();
                        inv._owner_id = reader.Current["owner_id"].As<string>();
                        inv._guests = new List<string>();
                        foreach (var guest in guests)
                        {
                            inv._guests.Add(guest["_email"].As<string>());
                        }

                        if (prods == null) continue;
                        inv._products = new List<OwnedProduct>();
                        foreach (var prod in prods)
                        {
                            var u = new OwnedProduct();
                            foreach (var (key, value) in prod["prod"].As<INode>().Properties)
                            {
                                u.GetType().GetProperty(key)?.SetValue(u, value, null);
                            }

                            u._consume_before = prod["expire"].As<DateTime>();
                            u._stock = prod["quant"].As<long>();

                            inv._products.Add(u);
                        }
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return inv;
        }

        public static async Task Add_prod(string uid, string prodName, int quant, DateTime date, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[]->(i:Inventory), (p:Product) " +
                                              "where u._email = $email and i.guid = $name and p._guid = $pguid " +
                                              "create (i)-[:CONTAIN {quantity: $quant, expiration_date: $date}]->(p)",
                        new {date, quant, email, name = uid, pguid = prodName});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task Restock(string uid, string prodName, string email, long? quant = null,
            LocalDateTime expire = null)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            if (quant == null && expire == null) return;
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var query = "Match (u:User)-[]->(i:Inventory)-[c:CONTAIN]->(p:Product) " +
                                "where u._email = $email and i.guid = $name and p._guid = $pguid ";
                    IDictionary<string, object> dic = new Dictionary<string, object>
                        {{"name", uid}, {"pguid", prodName}, {"email", email}};
                    if (quant != null)
                    {
                        query += "set c.quantity = $quant ";
                        dic.Add("quant", quant);
                    }

                    if (expire != null)
                    {
                        query += "set c.expiration_date = $date ";
                        dic.Add("date", expire);
                    }

                    var r = await tx.RunAsync(query, dic);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task RemoveProd(string uid, string prodName, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var query = "Match (u:User)-[]->(i:Inventory)-[c:CONTAIN]->(p:Product) " +
                                "where u._email = $email and i.guid = $name and p._guid = $pguid " +
                                "delete c";
                    IDictionary<string, object> dic = new Dictionary<string, object>
                        {{"name", uid}, {"pguid", prodName}, {"email", email}};

                    var r = await tx.RunAsync(query, dic);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task Share(string uid, string email, string friend)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:INV]->(i:Inventory), (z:User) " +
                                              "where u._email = $email and i.guid = $name and z._email = $friend " +
                                              "create (i)-[:Shared]->(z)", new {email, name = uid, friend});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<IDictionary<string, string>> GetAll(string email)
        {
            var l = new Dictionary<string, string>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)-[:INV]->(i:Inventory) " +
                                              "where u._email = $email " +
                                              "return i.name as name, i.guid as guid", new {email});
                    while (await r.FetchAsync())
                    {
                        l.Add(r.Current["name"].As<string>(), r.Current["guid"].As<string>());
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }

        public static async Task<IDictionary<string, string>> GetShared(string email)
        {
            var l = new Dictionary<string, string>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)<-[:Shared]-(i:Inventory) " +
                                              "where u._email = $email " +
                                              "return i.name as name, i.guid as guid", new {email});
                    while (await r.FetchAsync())
                    {
                        l.Add(r.Current["name"].As<string>(), r.Current["guid"].As<string>());
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }

        public static async Task EditName(string email, string uid, string new_name)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)-[:INV]->(i:Inventory) " +
                                              "where u._email = $email and i.guid = $uid " +
                                              "set i.name = $new_name", new {email, uid, new_name});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}