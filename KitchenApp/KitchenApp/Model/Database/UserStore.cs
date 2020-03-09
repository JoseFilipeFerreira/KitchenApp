using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Threading.Tasks;

namespace KitchenApp.Model.Database
{
    public class UserStore
    {
        public async Task<bool> user_exists(string uid)
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

        public async Task user_add(User u)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync("CREATE (:User {name: $name, passwd: $passwd, email: $email, birhdate: $bd})",
                        new {name = u._name, passwd = u._passwd, email = u._email, bd = u._birth_date});
                });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task user_rm(string uid)
        {
            var session = new Database("bolt://localhost:7687", "neo4j", "APPmvc").session();
            //var session = new Database("", "", "").session();
            try
            {
                await session.ReadTransactionAsync(async tx =>
                {
                    var lst = new List<string>();
                    var reader = await tx.RunAsync("MATCH(u:User) WHERE u.email = $email detach delete u",
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
    }
}