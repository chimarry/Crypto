using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.database.Entity
{
    class User
    {
      

        public User(string name, string password,string path,string folder)
        {
            Username = name;
            Password = password;
            CertPath = path;
            Folder = folder;
        }

        public string Username { set; get; }
        public string Password { set; get; }
        public string CertPath { set; get; }
        public string Folder { set; get; }
    }
}
