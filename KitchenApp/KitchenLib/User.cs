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
        public uint _phone_number { get; set; } 

        public User()
        {
        }

        public User(string name, string email, string passwd, LocalDateTime birthdate, uint phoneNumber)
        {
            _name = name;
            _email = email;
            _passwd = passwd;
            _birthdate = birthdate;
            _phone_number = phoneNumber;
        }

        public bool CheckPasswd(string passwd)
        {
            return _passwd.Equals(passwd);
        }
    }
}