using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Crypto.Cryptography
{
    class SymetricEncDec
    {
        public  enum Algs { AES,DES,IDEA }
        public static readonly Dictionary<String, IBlockCipher> ciphers = new Dictionary<string, IBlockCipher> { {Algs.AES.ToString(),new AesEngine() },
                                                                                                                { Algs.DES.ToString(),new DesEngine()},
                                                                                                                {Algs.IDEA.ToString(),new IdeaEngine() } };
        public static  readonly Dictionary<String, int> keyLenght = new Dictionary<String, int> { { Algs.AES.ToString(), 32 },
                                                                                 { Algs.DES.ToString(),8},
                                                                                 { Algs.IDEA.ToString(), 16},};
        public static byte[] Encrypt(byte[] dataForEncrypt,string alg,byte[] key)
        {
            return Crypto(true, dataForEncrypt, key, ciphers[alg]);
        }
        public static byte[] Decrypt(byte[] dataForDecrypt,string alg,byte[] key)
        {
            return Crypto(false, dataForDecrypt, key, ciphers[alg]);
        }
        private static byte[] Crypto(bool encryption,byte[] dataForCrypt,byte[] key,IBlockCipher blockCipher)
        {
            
                PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher);
                cipher.Init(encryption, new KeyParameter(key)); //does encryption or decryption based on value sent
                return cipher.DoFinal(dataForCrypt);
           
        }
    }
}
