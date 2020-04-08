using System;
using Neo4j.Driver;

namespace KitchenLib
{
    public class User
    {
        public string _name { get; set; }
        public string _email { get; set; }
        public string _passwd { get; set; }
        public LocalDateTime _birthdate { get; set; }

        public User()
        {
        }

        public bool CheckPasswd(string passwd)
        {
            return _passwd.Equals(passwd);
        }
    }
}