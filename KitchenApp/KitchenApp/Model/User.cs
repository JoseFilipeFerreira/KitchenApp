using System;

namespace KitchenApp.Model
{
    public class User
    {
        private string _name { get; set; }
        private string _email { get; set; }
        private string _passwd { get; set; }
        private DateTime _birthDate { get; set; }

        public User(string name, string email, string passwd, DateTime birthDate)
        {
            _name = name;
            _email = email;
            _passwd = passwd;
            _birthDate = birthDate;
        }
    }
}