using System;

namespace KitchenApp.Model
{
    public class User
    {
        public string _name { get; set; }
        public string _email { get; set; }
        public string _passwd { get; set; }
        public DateTime _birthdate { get; set; }

        public User(string name, string email, string passwd, DateTime birthdate)
        {
            _name = name;
            _email = email;
            _passwd = passwd;
            _birthdate = birthdate;
        }
    }
}