using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class RecipeStore
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
                    var reader = await tx.RunAsync("MATCH(u:Recipe) WHERE u.id = $guid RETURN u.id",
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

        public static async Task<MinimalRecipe> Get(string uid)
        {
            MinimalRecipe u = null;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:Recipe) Where u.id = $guid Return u",
                        new {guid = uid});
                    u = new MinimalRecipe();
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

        public static async Task Add(MinimalRecipe r)
        {
            MinimalRecipe u = null;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("CREATE (i:Recipe {title: $title, id: $id, image: $image}) " +
                                                   "return i",
                        new {r.id, r.image, r.title});
                    u = new MinimalRecipe();
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
        }

        public static async Task Star(string id, string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User) " +
                                                   "Match(z:Recipe) " +
                                                   "Where u._email = $email and z.id = $id " +
                                                   "Optional match (u)-[f:REC]-(z) " +
                                                   "with u, z, f, case when f is null then [1] else [] end as arr " +
                                                   "foreach(x in arr | create (u)-[:REC]->(z))",
                        new {email = user, id});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<List<MinimalRecipe>> GetStared(string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            var lst = new List<MinimalRecipe>();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)-[:REC]->(z:Recipe) " +
                                                   "where u._email = user " +
                                                   "return z",
                        new {email = user});
                    while (await reader.FetchAsync())
                    {
                        var r = new MinimalRecipe();

                        var aa = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in aa)
                        {
                            r.GetType().GetProperty(key)?.SetValue(r, value, null);
                        }

                        lst.Add(r);
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return lst;
        }

        public static async Task Unstar(string id, string user)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)-[r:REC]->(z:Recipe) " +
                                                   "where u._email = user and z.id = id " +
                                                   "delete r",
                        new {email = user, id});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}