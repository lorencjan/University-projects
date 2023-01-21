using System;
using System.Security.Authentication;
using System.Security.Cryptography;

namespace RockFests.DAL.StaticServices
{
    public static class Encryption
    {
        private const short HashSaltSize = 16;
        private const short HashSize = 32;

        public static void VerifyPasswords(string hashed, string entered)
        {
            const int remainingBits = HashSize - HashSaltSize;
            // Extract the bytes and get the salt from first
            var hashBytes = Convert.FromBase64String(hashed);
            var salt = new byte[HashSaltSize];
            Array.Copy(hashBytes, 0, salt, 0, HashSaltSize);
            // Compute the hash on the second password
            var pbkdf2 = new Rfc2898DeriveBytes(entered, salt, 10000);
            var hash = pbkdf2.GetBytes(remainingBits);
            // Compare the results
            for (var i = 0; i < remainingBits; i++)
            {
                if (hashBytes[i + HashSaltSize] != hash[i])
                    throw new InvalidCredentialException();
            }
        }

        public static string HashPassword(string password)
        {
            // Create the salt value with a cryptographic PRNG
            var salt = new byte[HashSaltSize];
            new RNGCryptoServiceProvider().GetBytes(salt);
            // Create the Rfc2898DeriveBytes and get the hash value:
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(HashSize - HashSaltSize);
            // Combine the salt and password bytes for later use:
            var hashBytes = new byte[HashSize];
            Array.Copy(salt, 0, hashBytes, 0, HashSaltSize);
            Array.Copy(hash, 0, hashBytes, HashSaltSize, HashSize - HashSaltSize);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
