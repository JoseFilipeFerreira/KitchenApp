using System;
using Neo4j.Driver;

namespace KitchenLib.Database
{
    public class Database : IDisposable
    {
        private readonly IDriver _driver;

        //TODO Read from a config file
        protected internal Database(string uri, string user, string passwd)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, passwd));
        }

        internal IAsyncSession session()
        {
            return _driver.AsyncSession();
        }
        
        public void Dispose()
        {
           _driver?.Dispose(); 
        }
    }
}