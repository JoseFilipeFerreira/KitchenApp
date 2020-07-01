using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class UserStore
    {
        public static async Task<bool> Exists(string uid)
        {
            Boolean exists;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User) WHERE u._email = $email RETURN u._email",
                        new {email = uid});
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

        public static async Task Add(User u)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("Merge (u:User {_email: $email}) " +
                                      "On Create set u._name = $name, u._passwd = $passwd, u._birthdate = $bd, u._phone_number = $ph " +
                                      "On Match set u._name = $name, u._passwd = $passwd, u._birthdate = $bd, u._phone_number = $ph ",
                        new
                        {
                            name = u._name, passwd = u._passwd, email = u._email, bd = u._birthdate,
                            ph = u._phone_number
                        });
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task<bool> Remove(string uid)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User)-[]->(i) WHERE u._email = $email detach delete u, i",
                        new {email = uid});
                    return reader.ConsumeAsync().Result.Counters.NodesDeleted != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        public static async Task<User?> Get(string uid)
        {
            User u = null;
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User) Where u._email = $email Return u",
                        new {email = uid});
                    while (await reader.FetchAsync())
                    {
                        var aa = reader.Current[0].As<INode>().Properties;
                        u = new User();
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

        public static async Task AddFriend(string uid, string friend)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User) " +
                                                   "Match(z:User) " +
                                                   "Where u._email = $email and z._email = $friend " +
                                                   "Optional match (u)-[f:FRND]-(z) " +
                                                   "with u, z, f, case when f is null then [1] else [] end as arr " +
                                                   "foreach(x in arr | create (u)-[:FRND {pending: true}]->(z))",
                        new {email = uid, friend});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public static async Task FriendshipRuined(string uid, string friend)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)-[f:FRND]-(z:User) " +
                                                   "Where u._email = $email and z._email = $friend " +
                                                   "delete f",
                        new {email = uid, friend});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
 
        public static async Task AcceptFriend(string uid, string friend)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)<-[f:FRND]-(z:User) " +
                                                   "Where u._email = $email and z._email = $friend " +
                                                   "set f.pending = false",
                        new {email = uid, friend});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
  
        public static async Task<IDictionary<string, string>> GetSentFriends(string uid)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            var pendings = new Dictionary<string, string>();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)-[f:FRND]->(z:User) " +
                                                   "Where u._email = $email and f.pending = true " +
                                                   "return z._email as email, z._name as name",
                        new {email = uid});
                    while (await reader.FetchAsync())
                    {
                        pendings.Add(reader.Current["email"].As<string>(), reader.Current["name"].As<string>());
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return pendings;
        }
  
        public static async Task<IDictionary<string, string>> GetPendingFriends(string uid)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            var pendings = new Dictionary<string, string>();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)<-[f:FRND]-(z:User) " +
                                                   "Where u._email = $email and f.pending = true " +
                                                   "return z._email as email, z._name as name",
                        new {email = uid});
                    while (await reader.FetchAsync())
                    {
                        pendings.Add(reader.Current["email"].As<string>(), reader.Current["name"].As<string>());
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return pendings;
        }
          
        public static async Task<IDictionary<string, string>> GetFriends(string uid)
        {
            var session = new Database("bolt://db:7687", "neo4j", "APPmvc").session();
            var pendings = new Dictionary<string, string>();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync("Match(u:User)-[f:FRND]-(z:User) " +
                                                   "Where u._email = $email and f.pending = false " +
                                                   "return z._email as email, z._name as name",
                        new {email = uid});
                    while (await reader.FetchAsync())
                    {
                        pendings.Add(reader.Current["email"].As<string>(), reader.Current["name"].As<string>());
                    }
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return pendings;
        }
    }
}