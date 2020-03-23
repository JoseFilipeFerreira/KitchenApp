using System;
using Neo4j.Driver;

namespace KitchenApp.Model.Database
{
    public class Database : IDisposable
    {
        private readonly IDriver _driver;

        //TODO Read from a config file
        Database(string uri, string user, string passwd)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, passwd));
        }

        public void Dispose()
        {
           _driver?.Dispose(); 
        }
    }
}