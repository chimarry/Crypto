using Crypto.database.Entity;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;

namespace Crypto.Cryptography
{
    class AsymmetricEncDec
    {
        public static string crlName = "Vanja.pem";
        public static readonly string PASSWORD="marija";
        public static byte[] SignData(byte[] input, User user,string hashAlg)
        {   
            if(UsageHasDigitalSignature(user) == false)
            {
                
                throw new KeyException();
            }
            X509Certificate2 certificate = new X509Certificate2(user.CertPath,PASSWORD);
            RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)certificate.PrivateKey;
            return privateKey.SignHash(input, CryptoConfig.MapNameToOID(hashAlg));
            
        }
        public static bool VerifySignature(byte[] input, User user,byte[] result,string hashAlg)
        {
            X509Certificate2 certificate = new X509Certificate2(user.CertPath,PASSWORD);
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            return publicKey.VerifyHash(result, CryptoConfig.MapNameToOID(hashAlg), input);

        }
        public static bool CheckValidation(User user)
        {
            bool value;
            string CrlPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Certs\\" + crlName;
            CrlPath=new Uri(CrlPath).LocalPath;
            X509CrlParser parser = new X509CrlParser();
            using (BinaryReader reader = new BinaryReader(File.Open(CrlPath, FileMode.Open)))
            {
                X509Crl crlData = parser.ReadCrl(reader.ReadBytes((int)new FileInfo(CrlPath).Length));
                X509Certificate2 certificate = new X509Certificate2(user.CertPath,PASSWORD);
                var bouncyCert = DotNetUtilities.FromX509Certificate(certificate);
                value = crlData.IsRevoked(bouncyCert);
            }
            return value;
        }
        public static byte[] EncryptInformation(User user, byte[] data)
        {
            if (UsageHasKeyEncipherment(user) == false)
            {
                throw new KeyException();
            }
            X509Certificate2 certificate = new X509Certificate2(user.CertPath,PASSWORD);
            RSACryptoServiceProvider publicKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            return publicKey.Encrypt(data, false);
        }
        public static byte[] DecryptInformation(User user, byte[] data)
        {
            X509Certificate2 certificate = new X509Certificate2(user.CertPath,PASSWORD);
            RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider)certificate.PrivateKey;
            return privateKey.Decrypt(data, false);

        }
        static public bool UsageHasKeyEncipherment(User user)
        {
            X509Certificate2 tmpCert = new X509Certificate2(user.CertPath, PASSWORD);
            foreach (X509KeyUsageExtension usage_extension in tmpCert.Extensions.OfType<X509KeyUsageExtension>())
            {
                if ((usage_extension.KeyUsages & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment)
                {
                    return true;
                }
            }
            return false;
        }
        static public bool UsageHasDigitalSignature(User user)
        {
            X509Certificate2 tmpCert = new X509Certificate2(user.CertPath, PASSWORD);
            foreach (X509KeyUsageExtension usage_extension in tmpCert.Extensions.OfType<X509KeyUsageExtension>())
            {
                if ((usage_extension.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
