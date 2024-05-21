using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Utility
{
    public class DigitalSignature
    {
        private ECDsa ecdsa;

        public DigitalSignature()
        {
            ecdsa = ECDsa.Create();
        }

        public string ExportPublicKey()
        {
            var parameters = ecdsa.ExportParameters(false);
            return Convert.ToBase64String(ecdsa.ExportSubjectPublicKeyInfo());
        }

        public void ImportPublicKey(string publicKey)
        {
            byte[] publicKeyBytes = Convert.FromBase64String(publicKey);
            ecdsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
        }

        public string SignData(byte[] data)
        {
            var signedData = ecdsa.SignData(data, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(signedData);
        }

        public bool VerifyData(byte[] data, string signature)
        {
            var signatureBytes = Convert.FromBase64String(signature);
            return ecdsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA256);
        }
    }
}
