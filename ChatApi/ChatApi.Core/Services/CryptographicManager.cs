using ChatApi.Core.Interfaces;
using System.Security.Cryptography;


namespace ChatApi.Core.Services
{
    public  class CryptographicManager : ICryptographicManager
    {
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
        public byte[] Decrypt(byte[] dataToDecrypt, byte[] key)
        {
            using (var aesCbc = Aes.Create())
            {
                aesCbc.Mode = CipherMode.CBC;
                aesCbc.Padding = PaddingMode.PKCS7;

                var iv = new byte[aesCbc.BlockSize / 8];
                Buffer.BlockCopy(dataToDecrypt, 0, iv, 0, iv.Length);

                var actualData = new byte[dataToDecrypt.Length - iv.Length];
                Buffer.BlockCopy(dataToDecrypt, iv.Length, actualData, 0, actualData.Length);

                aesCbc.Key = key;
                aesCbc.IV = iv;

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


        public byte[] Encrypt(byte[] dataToEncrypt, byte[] key)
        {
            using (var aesCbc = Aes.Create())
            {
                aesCbc.Key = key;
                aesCbc.Mode = CipherMode.CBC;
                aesCbc.Padding = PaddingMode.PKCS7;

                aesCbc.GenerateIV();
                byte[] iv = aesCbc.IV;

                byte[] encryptedData;

                using (var encryptor = aesCbc.CreateEncryptor(aesCbc.Key, iv))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                    }
                    encryptedData = msEncrypt.ToArray();
                }

                var combinedIvEncData = new byte[iv.Length + encryptedData.Length];
                Buffer.BlockCopy(iv, 0, combinedIvEncData, 0, iv.Length);
                Buffer.BlockCopy(encryptedData, 0, combinedIvEncData, iv.Length, encryptedData.Length);

                return combinedIvEncData;
            }
        }
    }
}
