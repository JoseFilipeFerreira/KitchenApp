using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace KitchenApp.Model.Database
{
    public class UserStore
    {
        public async Task<bool> Exists(string uid)
        {
            Boolean exists = false;
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            //var session = new Database("", "", "").session();
            try
            {
                exists = await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User) WHERE u.email = $email RETURN u.email",
                        new {email = uid});
                    while (await reader.FetchAsync())
                        lst.Add(reader.Current[0].ToString());

                    Console.WriteLine(lst.Count);
                    return lst.Count != 0;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return exists;
        }

        public async Task Add(User u)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("CREATE (:User {_name: $name, _passwd: $passwd, _email: $email, _birhdate: $bd})",
                        new {name = u._name, passwd = u._passwd, email = u._email, bd = u._birthdate});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task Remove(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User) WHERE u._email = $email detach delete u",
                        new {email = uid});
                    while (await reader.FetchAsync())
                        lst.Add(reader.Current[0].ToString());

                    Console.WriteLine(lst.Count);
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<User> Get(String uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("Match(u:User) Where u._email = $email Return u",
                        new {email=uid});
                    var user = new User("", "", "", DateTime.Now);
                    while (await reader.FetchAsync())
                    {
                        var aa = reader.Current[0].As<INode>().Properties;
                        foreach (var (key, value) in aa)
                        {
                            user.GetType().GetProperty(key)?.SetValue(user, value, null);
                        }
                    }

                    return user;
                });
            }
            finally
            {
                await session.CloseAsync();
            }

            return null;
        }
    }
}