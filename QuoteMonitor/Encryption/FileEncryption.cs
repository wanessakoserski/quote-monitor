using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuoteMonitor.Encryption
{
    internal class FileEncryption
    {
        private static readonly byte[] Key =
            Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        private static readonly byte[] Iv =
            Encoding.UTF8.GetBytes("1234567890123456");

        public static void EncryptFile(
            string sourcePath,
            string destinationPath)
        {
            var content = File.ReadAllText(sourcePath);

            using var aes = Aes.Create();

            aes.Key = Key;
            aes.IV = Iv;

            using var destinationStream =
                new FileStream(destinationPath, FileMode.Create);

            using var cryptoStream = new CryptoStream(
                destinationStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write);

            using var writer = new StreamWriter(cryptoStream);

            writer.Write(content);
        }

        public static string DecryptFile(string encryptedPath)
        {
            using var aes = Aes.Create();

            aes.Key = Key;
            aes.IV = Iv;

            using var sourceStream =
                new FileStream(encryptedPath, FileMode.Open);

            using var cryptoStream = new CryptoStream(
                sourceStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            using var reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
    }
}
