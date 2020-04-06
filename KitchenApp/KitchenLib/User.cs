using System;

namespace KitchenLib
{
    public class User
    {
        public string _name { get; set; }
        public string _email { get; set; }
        public string _passwd { get; set; }
        public DateTime _birthday { get; set; }

        public User(string name, string email, string passwd, DateTime birthday)
        {
            _name = name;
            _email = email;
            _passwd = passwd;
            _birthday = birthday;
        }

        public bool CheckPasswd(string passwd)
        {
            return _passwd.Equals(passwd);
        }
    }
}