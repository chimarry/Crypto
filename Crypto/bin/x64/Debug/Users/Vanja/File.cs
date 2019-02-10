using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoProject.Common
{
    public sealed class HashingPassword
    {
        public static string Hash(string password)
        {

            using (SHA1 sha1 = SHA1.Create())
            {
                string result = GetSHA1Hash(sha1, password);
                return result;
            }
        }

        private static string GetSHA1Hash(SHA1 sha1, string password)
        {
            byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (var x in data)
                sb.Append(x.ToString("x2"));
            return sb.ToString();
        }
    }    
}
