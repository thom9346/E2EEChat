using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ChatApi.Core.Utility
{
    public static class PasswordHasher
    {
        private static string _pepper;

        public static void Initialize(string pepper)
        {
            _pepper = pepper 
                ?? throw new ArgumentNullException(nameof(pepper));
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password + _pepper,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public static bool VerifyPassword(string hashedPasswordWithSalt, string passwordToCheck)
        {
            if (string.IsNullOrEmpty(_pepper))
                throw new InvalidOperationException("Pepper not initialized");


            var parts = hashedPasswordWithSalt.Split('.', 2);
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hashedPassword = parts[1];

            string checkHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passwordToCheck + _pepper,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashedPassword == checkHashed;
        }
    }
}
