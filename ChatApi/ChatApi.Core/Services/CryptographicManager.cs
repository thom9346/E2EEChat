using ChatApi.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ChatApi.Core.Services
{
    public class CryptographicManager : ICryptographicManager
    {
        private readonly byte[] _key;

        public CryptographicManager(IConfiguration configuration)
        {
            var keyString = configuration["Encryption:Key"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length != 32)
            {
                throw new InvalidOperationException("Invalid encryption key.");
            }
            _key = Encoding.UTF8.GetBytes(keyString);
        }

        public byte[] CryptographicRandomNumberGenerator(int length)
        {
            byte[] randomBytes = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public byte[] DeriveKey(string password, byte[] salt, int iterations, int keySize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                return pbkdf2.GetBytes(keySize);
            }
        }

        public byte[] Encrypt(byte[] dataToEncrypt, out byte[] iv)
        {
            using (var aesCbc = Aes.Create())
            {
                aesCbc.Key = _key;
                aesCbc.Mode = CipherMode.CBC;
                aesCbc.Padding = PaddingMode.PKCS7;

                aesCbc.GenerateIV();
                iv = aesCbc.IV;

                byte[] encryptedData;

                using (var encryptor = aesCbc.CreateEncryptor(aesCbc.Key, iv))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    encryptedData = msEncrypt.ToArray();
                }

                var combinedIvEncData = new byte[iv.Length + encryptedData.Length];
                Buffer.BlockCopy(iv, 0, combinedIvEncData, 0, iv.Length);
                Buffer.BlockCopy(encryptedData, 0, combinedIvEncData, iv.Length, encryptedData.Length);

                return combinedIvEncData;
            }
        }

        public byte[] Decrypt(byte[] dataToDecrypt, byte[] iv)
        {
            using (var aesCbc = Aes.Create())
            {
                aesCbc.Key = _key;
                aesCbc.Mode = CipherMode.CBC;
                aesCbc.Padding = PaddingMode.PKCS7;

                aesCbc.IV = iv;

                var actualData = new byte[dataToDecrypt.Length - iv.Length];
                Buffer.BlockCopy(dataToDecrypt, iv.Length, actualData, 0, actualData.Length);

                using (var decryptor = aesCbc.CreateDecryptor(aesCbc.Key, aesCbc.IV))
                using (var msDecrypt = new MemoryStream(actualData))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    var decryptedData = new List<byte>();
                    int b;
                    while ((b = csDecrypt.ReadByte()) != -1)
                    {
                        decryptedData.Add((byte)b);
                    }
                    return decryptedData.ToArray();
                }
            }
        }
    }
}
