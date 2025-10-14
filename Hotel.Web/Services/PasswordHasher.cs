using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Services
{
    using BCrypt.Net;

    public class PasswordHasher
    {
        // Hasher un mot de passe
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, BCrypt.GenerateSalt(12)); // 12 est le facteur de coût (plus il est élevé, plus le hachage est sécurisé)
        }

        // Vérifier un mot de passe
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }
}
