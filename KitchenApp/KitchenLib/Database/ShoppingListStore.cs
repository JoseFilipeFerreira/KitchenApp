using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class ShoppingListStore
    {
        public static async Task<bool> Exists(string uid, string email)
        {
            var exists = false;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[]-(i:Shoppinglist) WHERE u._email = $email AND i.guid = $uid RETURN i.name",
                        new {email, uid});
                    while (await reader.FetchAsync())
                    {
                        lst.Add(reader.Current[0].ToString());
                    }

                    exists = lst.Count != 0;
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }

            return exists;
        }

        public static async Task<bool> ExistName(string name, string email)
        {
            var exists = false;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync(
                        "MATCH(u:User)-[:SHP]->(i:Shoppinglist) WHERE u._email = $email AND i.name = $name RETURN i.name",
                        new {email, name});
                    while (await reader.FetchAsync())
                        lst.Add(reader.Current[0].ToString());

                    return lst.Count != 0;
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }

            return exists;
        }

        public static async Task Add(Inventory<WantedProduct> inv, string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("MATCH (u:User) where u._email = $user " +
                                      "CREATE (i:Shoppinglist {name: $name, guid: $guid}) " +
                                      "CREATE (u)-[:SHP]->(i)",
                        new {user, name = inv._name, guid = inv._guid});
                });
            }
            catch
            {
                // ignored
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
                    var reader = await tx.RunAsync("MATCH(u:User)-[:SHP]->(i:Shoppinglist)" +
                                                   "WHERE u._email = $email AND i.guid = $name " +
                                                   "detach delete i",
                        new {email, name = uid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public static async Task<Inventory<WantedProduct>> Get(string uid, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            Inventory<WantedProduct> inv = null;
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync(
                        "Match(u:User)-[]-(i:Shoppinglist) " +
                        "Where u._email = $email AND i.guid = $name " +
                        "Return [(i)-[c:CONTAIN]->(p) where p: Product | { prod: p, quant: c.quantity }] as products, " +
                        "[(i)-[:Shared]-(b) where b: User | b] as guests, " +
                        "u._email as owner_id, " +
                        "i.name as name, i.guid as guid",
                        new {email, name = uid});
                    while (await reader.FetchAsync())
                    {
                        inv = new Inventory<WantedProduct>();
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
                        inv._products = new List<WantedProduct>();
                        foreach (var prod in prods)
                        {
                            var u = new WantedProduct();
                            foreach (var (key, value) in prod["prod"].As<INode>().Properties)
                            {
                                u.GetType().GetProperty(key)?.SetValue(u, value, null);
                            }

                            u._stock = prod["quant"].As<uint>();

                            inv._products.Add(u);
                        }
                    }
                });
                return inv;
            }
            catch
            {
                return null;
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task Add_prod(string email, string uid, string prodName, int quant)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[]-(i:Shoppinglist), (p:Product) " +
                                              "where u._email = $email and i.guid = $name and p._guid = $pguid " +
                                              "Optional match (i)-[f:CONTAIN]-(p) " +
                                              "with i, p, f, case when f is null then [1] else [] end as arr " +
                                              "foreach(x in arr | create (i)-[:CONTAIN {quantity: $quant}]->(p))",
                        new {quant, email, name = uid, pguid = prodName});
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task Restock(string uid, string prodName, long quant, string email)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync(
                        "Match (u:User)-[]-(i:Shoppinglist)-[c:CONTAIN]->(p:Product) " +
                        "where u._email = $email and i.guid = $name and p._guid = $pguid " +
                        "Set c.quantity = $quant", new {name = uid, pguid = prodName, email, quant});
                });
            }
            catch
            {
                // ignored
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
                    var query = "Match (u:User)-[]-(i:Shoppinglist)-[c:CONTAIN]->(p:Product) " +
                                "where u._email = $email and i.guid = $name and p._guid = $pguid " +
                                "delete c";
                    IDictionary<string, object> dic = new Dictionary<string, object>
                        {{"name", uid}, {"pguid", prodName}, {"email", email}};

                    var r = await tx.RunAsync(query, dic);
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<IDictionary<string, string>> GetShared(string email)
        {
            var l = new Dictionary<string, string>();
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("match (u:User)-[:Shared]-(i:Shoppinglist) " +
                                              "where u._email = $email " +
                                              "return i.name as name, i.guid as guid", new {email});
                    while (await r.FetchAsync())
                    {
                        l.Add(r.Current["name"].As<string>(), r.Current["guid"].As<string>());
                    }
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }

            return l;
        }

        public static async Task Share(string uid, string email, string friend)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var r = await tx.RunAsync("Match (u:User)-[:SHP]->(i:Shoppinglist), (z:User) " +
                                              "where u._email = $email and i.guid = $name and z._email = $friend " +
                                              "Optional match (i)-[f:Shared]-(z) " +
                                              "with i, z, f, case when f is null then [1] else [] end as arr " +
                                              "foreach(x in arr | create (i)<-[:Shared]-(z))",
                                              new {email, name = uid, friend});
                });
            }
            catch
            {
                // ignored
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
                    var r = await tx.RunAsync("match (u:User)-[:SHP]->(i:Shoppinglist) " +
                                              "where u._email = $email " +
                                              "return i.name as name, i.guid as guid", new {email});
                    while (await r.FetchAsync())
                    {
                        l.Add(r.Current["name"].As<string>(), r.Current["guid"].As<string>());
                    }
                });
            }
            catch
            {
                // ignored
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
                    var r = await tx.RunAsync("match (u:User)-[:SHP]->(i:Shoppinglist) " +
                                              "where u._email = $email and i.guid = $uid " +
                                              "set i.name = $new_name", new {email, uid, new_name});
                });
            }
            catch
            {
                // ignored
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}