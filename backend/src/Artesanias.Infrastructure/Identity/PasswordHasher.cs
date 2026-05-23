using BCrypt.Net;

namespace Artesanias.Infrastructure.Identity;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: HashType.SHA384);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash, hashType: HashType.SHA384);
    }
}
