using Common;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Services.Utils
{
    public interface IPasswordHashingService
    {
        string Hash(string password);
    }
    
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly byte[] salt = "SecretKey".GetBytes();
        
        public string Hash(string password)
        {
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100, 20).AsString();
        }
    }
}