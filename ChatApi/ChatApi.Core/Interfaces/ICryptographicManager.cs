using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Interfaces
{
    public interface ICryptographicManager
    {
        byte[] CryptographicRandomNumberGenerator(int length);
        byte[] DeriveKey(string password, byte[] salt, int iterations, int keySize);
        byte[] Encrypt(byte[] dataToEncrypt, out byte[] iv);
        byte[] Decrypt(byte[] dataToDecrypt, byte[] iv);

    }
}
