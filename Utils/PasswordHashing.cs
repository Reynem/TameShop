using System.Security.Cryptography;


namespace TameShop.Utils
{
    public class PasswordHashing
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName HashingAlgorithm = HashAlgorithmName.SHA256;

        public static string HashPassword(string password)
        {
            byte[] salt;
            using (var rng = RandomNumberGenerator.Create())
            {
                salt = new byte[SaltSize];
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, hashAlgorithm: HashingAlgorithm)) // Fixed CS0120 by using static field
            {
                hash = pbkdf2.GetBytes(SaltSize);
            }

            byte[] hashBytes = new byte[SaltSize + SaltSize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, SaltSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHashedPassword);
            if (hashBytes.Length != (SaltSize + SaltSize))
            {
                return false;
            }

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            byte[] storedHash = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedHash, 0, SaltSize);

            byte[] computedHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashingAlgorithm)) 
            {
                computedHash = pbkdf2.GetBytes(SaltSize);
            }

            return SlowEquals(computedHash, storedHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }

    }
}
