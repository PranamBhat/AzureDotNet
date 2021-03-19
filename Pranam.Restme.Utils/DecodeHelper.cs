using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Pranam
{
    public class DecodeHelper
    {
        public static string DESDecrypt(string pToDecrypt, byte[] sKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            return UTF8Encoding.UTF8.GetString(DESDecrypt(inputByteArray, sKey)).Trim();
        }

        public static byte[] DESDecrypt(byte[] pToDecrypt, byte[] sKey)
        {
            using (TripleDES des = TripleDES.Create())
            {
                des.Padding = PaddingMode.PKCS7;
                des.Key = sKey;
                des.IV = sKey;
                byte[] returnValue = null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(pToDecrypt, 0, pToDecrypt.Length);
                        cs.FlushFinalBlock();
                    }

                    returnValue = ms.ToArray();
                }

                return returnValue;
            }
        }


        /// <summary>
        /// Decrypts input string from Rijndael (AES) algorithm with CBC blocking and PKCS7 padding.
        /// </summary>
        /// <param name="inputBytes">Encrypted binary array to decrypt</param>
        /// <returns>string of Decrypted data</returns>
        /// <remarks>The key and IV are the same, and use SagePaySettings.EncryptionPassword.</remarks>
        public static string AESDecrypt(byte[] inputBytes, string encryptionPassword)
        {
            RijndaelManaged AES = new RijndaelManaged();
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
            byte[] keyAndIvBytes = encoding.GetBytes(encryptionPassword);
            byte[] outputBytes = new byte[inputBytes.Length + 1];

            //set the mode, padding and block size
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CBC;
            AES.KeySize = 128;
            AES.BlockSize = 128;

            //create streams and decryptor object
            dynamic memoryStream = new MemoryStream(inputBytes);
            dynamic cryptoStream = new CryptoStream(memoryStream, AES.CreateDecryptor(keyAndIvBytes, keyAndIvBytes),
                CryptoStreamMode.Read);

            //perform decryption
            cryptoStream.Read(outputBytes, 0, outputBytes.Length);

            //close streams
            memoryStream.Close();
            cryptoStream.Close();
            AES.Clear();

            //convert decryted data into string, assuming original text was UTF-8 encoded
            return encoding.GetString(outputBytes);
        }
    }
    
}