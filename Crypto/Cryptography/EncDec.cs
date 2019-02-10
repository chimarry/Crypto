using Crypto.database.Entity;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using System.Windows;

namespace Crypto.Cryptography
{
    class EncDec
    {
        public static User sender;
        public static User receiver;

        public static readonly byte ZERO_BYTE = 0;
        public static byte[] RandomKeyGenerator(int keyLength) {
            byte[] data = new byte[keyLength];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            return data;
        }

        public static bool Encrypt(string algorithm, string hashAlg, string filePath)
        {
            
            byte[] key = RandomKeyGenerator(SymetricEncDec.keyLenght[algorithm]);
            //get data 
            byte[] dataForEncryption = ReadFile(filePath);
            byte[] dataEncrypted=SymetricEncDec.Encrypt(dataForEncryption, algorithm, key);
            //get hash
            byte[] hash = HashAlg.Hash(dataForEncryption, hashAlg);
            hash = AsymmetricEncDec.SignData(hash, sender,hashAlg);

            WriteWithHeader(algorithm,hashAlg,key,hash.Length,dataEncrypted,hash,receiver.Folder+"\\"+sender.Username+new Random().Next(0,100)+".cs");
           
            return true;
        }
        public static string Decrypt(string filePath)
        {
            byte[] data;
            byte[] symetricAlg;
            byte[] key;
            byte[] hash;
            byte[] hashingAlg;
            int hashLength;
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                try
                {
                    data = reader.ReadBytes((int)(new System.IO.FileInfo(filePath).Length));
                    data = ReadAndSkip(data,out symetricAlg);
                    data = ReadAndSkip(data, out hashingAlg);
                    data = ReadAndSkip(data, out key);

                }
                catch (Exception)
                {
                    MessageBox.Show("Decryption with your private key failed.");
                    return null;
                }
                //get signed hash
                hashLength = BitConverter.ToInt32(data.Take(4).ToArray(),0);
                data = data.Skip(4).ToArray();
                hash = data.Take(hashLength).ToArray();
                
                //get encrypted data
                data = data.Skip(hashLength).ToArray();
            }
            byte[] dataDecrypted = SymetricEncDec.Decrypt(data, Encoding.UTF8.GetString(symetricAlg), key);
            byte[] result = HashAlg.Hash(dataDecrypted, Encoding.UTF8.GetString(hashingAlg));
            if (AsymmetricEncDec.VerifySignature(hash, sender, result, Encoding.UTF8.GetString(hashingAlg)) ==false)
                return null;
            string filePathOut = filePath.Remove(filePath.LastIndexOf('.')) + "Dec.cs";
            WriteToFile(filePathOut, dataDecrypted);
            return filePathOut;
        }
        private static byte[] ReadAndSkip(byte[] data,out byte[] readBytes)
        {
            int length = BitConverter.ToInt32(data.Take(4).ToArray(), 0);
            data = data.Skip(4).ToArray();
            readBytes = data.Take(length).ToArray();
            data = data.Skip(readBytes.Length).ToArray();
            readBytes = AsymmetricEncDec.DecryptInformation(receiver, readBytes);
            return data;
        }
        public static void WriteWithHeader(string algorithm,string hashAlg, byte[] key,int hashLength, byte[] data,byte[] hash, string file) //inserts data with header to specified file
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Create)))
            {//write used algorithm
                WriteAndAdd(Encoding.UTF8.GetBytes(algorithm), writer);
                WriteAndAdd(Encoding.UTF8.GetBytes(hashAlg), writer);
                WriteAndAdd(key, writer);
             //write signed hash
                writer.Write(hashLength);
                WriteHeader(hash,writer);
            //write crypted data
                writer.Write(data);
            }
        }
        private static void WriteAndAdd(byte[] data, BinaryWriter writer)
        {
            byte[] tmp = AsymmetricEncDec.EncryptInformation(receiver, data);
            writer.Write(tmp.Length);
            WriteHeader(tmp, writer);
        }
        private static void WriteHeader(byte[] word,BinaryWriter writer)
        {
            writer.Write(word);
        }
        private static byte[] ReadFile(string file) {
            byte[] data;
            using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
               data = reader.ReadBytes((int)(new System.IO.FileInfo(file).Length));
            return data;
        }
        private static void WriteToFile(string fileName,byte[] data)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate)))
                writer.Write(data);
        }
       
    }
}
