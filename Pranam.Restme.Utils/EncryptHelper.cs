using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Pranam
{
    public partial class EncryptHelper
    {
        public static string MD5Encrypt(string valueString)
        {
            string ret = String.Empty;
            var md5Hasher = MD5.Create();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(valueString);
            data = md5Hasher.ComputeHash(data);
            for (int i = 0; i < data.Length; i++)
            {
                ret += data[i].ToString("x2").ToLower();
            }

            return ret;
        }

        public static byte[] MD5Encrypt(byte[] sourceToEncrypt)
        {
            Byte[] encodedBytes;
            MD5 md5 = MD5.Create();
            encodedBytes = md5.ComputeHash(sourceToEncrypt);
            return encodedBytes;
        }

        public static byte[] GetDESKey()
        {
            var des = TripleDES.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.GenerateKey();
            return des.Key;
        }

        public static string DESEncrypt(string pToEncrypt, byte[] sKey)
        {
            byte[] inputByteArray = UTF8Encoding.UTF8.GetBytes(pToEncrypt);
            return Convert.ToBase64String(DESEncrypt(inputByteArray, sKey));
        }

        public static byte[] DESEncrypt(byte[] pToEncrypt, byte[] sKey)
        {
            byte[] returnValue = null;
            using (var des = TripleDES.Create())
            {
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                byte[] inputByteArray = pToEncrypt;
                des.Key = sKey;
                des.IV = sKey;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }

                    returnValue = ms.ToArray();
                }
            }

            return returnValue;
        }

        public static TripleDES GetDESEncryptor()
        {
            var des = TripleDES.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            return des;
        }

        public static string GetHMACSHA256(string key, string message)
        {
            var keyByte = Encoding.UTF8.GetBytes(key);
            using (var hmacSha256 = new HMACSHA256(keyByte))
            {
                hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));

                return ByteToString(hmacSha256.Hash);
            }
        }

        private static string ByteToString(IEnumerable<byte> buff)
        {
            return buff.Aggregate("", (current, t) => current + t.ToString("X2"));
        }


        /// <summary>
        /// Encrypts input string using Rijndael (AES) algorithm with CBC blocking and PKCS7 padding.
        /// </summary>
        /// <param name="inputText">text string to encrypt </param>
        /// <returns>Encrypted text in Byte array</returns>
        /// <remarks>The key and IV are the same, in this method - using encryptionPassword.</remarks>
        public static byte[] AESEncrypt(string inputText, string encryptionPassword)
        {
            RijndaelManaged AES = new RijndaelManaged();
            byte[] outBytes = null;

            //set the mode, padding and block size for the key
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CBC;
            AES.KeySize = 128;
            AES.BlockSize = 128;

            //convert key and plain text input into byte arrays
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            byte[] keyAndIvBytes = encoding.GetBytes(encryptionPassword);
            byte[] inputBytes = encoding.GetBytes(inputText);

            //create streams and encryptor object
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                AES.CreateEncryptor(keyAndIvBytes, keyAndIvBytes), CryptoStreamMode.Write);

            //perform encryption
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();

            //get encrypted stream into byte array
            outBytes = memoryStream.ToArray();

            //close streams
            memoryStream.Close();
            cryptoStream.Close();
            AES.Clear();

            return outBytes;
        }

    }
}