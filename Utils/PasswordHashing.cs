using System.Security.Cryptography;


namespace TameShop.Utils
{
    public class PasswordHashing
    {
        private const int SaltSize = 16; 
        private const int HashSize = 32;
        private const int Iterations = 10000;
        private readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;

        public string HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[SaltSize];
                rng.GetBytes(salt);
                using (var hashAlgorithm = new Rfc2898DeriveBytes(password, salt, Iterations, hashAlgorithmName))
                {
                    var hash = hashAlgorithm.GetBytes(HashSize);
                    return Convert.ToBase64String(salt.Concat(hash).ToArray());
                }
            }
        }
    }
}
