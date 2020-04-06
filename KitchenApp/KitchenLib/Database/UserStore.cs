using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class UserStore
    {
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

        //Updates user if ``CREATE CONSTRAINT ON (n:User) ASSERT n.email IS UNIQUE``
        public async Task Add(User u)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("CREATE (:User {_name: $name, _passwd: $passwd, _email: $email, _birthday: $bd})",
                        new {name = u._name, passwd = u._passwd, email = u._email, bd = u._birthday});
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
                    var reader = await tx.RunAsync("MATCH(u:User) WHERE u._email = $email detach delete u",
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
        
        public async Task<User> Get(String uid)
        {
            User u = null;
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("Match(u:User) Where u._email = $email Return u",
                        new {email=uid});
                    u = new User("", "", "", DateTime.Now);
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
    }
}