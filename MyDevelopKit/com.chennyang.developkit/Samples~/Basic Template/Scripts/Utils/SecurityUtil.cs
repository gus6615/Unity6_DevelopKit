using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevelopKit.BasicTemplate
{
    public static class SecurityUtil
    {
        private static readonly string _xorKey = "uArVrkeAeCfeOndqNdrdHyRjeH3XEMr8";
        private static readonly string _aesKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("R4ADa1K14Wuy4oWza01SJfdverwnC7Pl".PadRight(32, '0')));
        private static readonly string _aesIv = Convert.ToBase64String(Encoding.UTF8.GetBytes("ONZjdMmli4TfDciQ".PadRight(16, '0')));
        
        public static string XorEncryptAndDecrypt(string input)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (Byte)(bytes[i] ^ _xorKey[i % _xorKey.Length]);
            }
            return Encoding.ASCII.GetString(bytes);
        }
        
        public static string Aes256Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Convert.FromBase64String(_aesKey);
                aes.IV = Convert.FromBase64String(_aesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Aes256Decrypt(string encryptedText)
        {
            string decrypted;
            byte[] cipher = Convert.FromBase64String(encryptedText);
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = Convert.FromBase64String(_aesKey);
                aes.IV = Convert.FromBase64String(_aesIv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new(cipher))
                {
                    using (CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new(cs))
                        {
                            decrypted = sr.ReadToEnd();
                        }
                    }
                }
            }

            return decrypted;
        }
    }
}