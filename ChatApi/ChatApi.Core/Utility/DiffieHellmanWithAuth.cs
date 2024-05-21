using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Utility
{
    public class DiffieHellmanWithAuth
    {
        private ECDiffieHellmanCng dh;
        public byte[] PublicKey { get; private set; }

        public DiffieHellmanWithAuth()
        {
            dh = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };
            PublicKey = dh.PublicKey.ToByteArray();
        }

        public byte[] DeriveSharedSecret(byte[] otherPartyPublicKey)
        {
            var otherKey = ECDiffieHellmanCngPublicKey.FromByteArray(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob);
            return dh.DeriveKeyMaterial(otherKey);
        }
    }
}
