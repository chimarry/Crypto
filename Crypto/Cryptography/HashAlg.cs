using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Crypto.Cryptography
{
    class HashAlg
    {
        public enum Algs { SHA1, MD5};
        public static Dictionary<String, HashAlgorithm> digests = new Dictionary<string, HashAlgorithm> { { Algs.SHA1.ToString(), new SHA1Managed() },
                                                                                { Algs.MD5.ToString(), new MD5Cng() } };
        public static byte[] Hash(byte[] input, string alg)
        {
           HashAlgorithm digest = digests[alg];
           return digest.ComputeHash(input);
        }
    }
}
