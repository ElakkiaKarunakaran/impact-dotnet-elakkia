using System.Security.Cryptography;

namespace Identity.Services;

// Task 10.2 — PBKDF2 (RFC 2898) with a per-user random salt.
// 100 000 iterations is a reasonable 2026 baseline; raise it when hardware does.
public static class PasswordHasher
{
    private const int SaltSize   = 16;
    private const int HashSize   = 32;
    private const int Iterations = 100_000;

    public static (string hash, string salt) Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password, saltBytes, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public static bool Verify(string password, string hashB64, string saltB64)
    {
        var saltBytes     = Convert.FromBase64String(saltB64);
        var expectedBytes = Convert.FromBase64String(hashB64);
        var actualBytes   = Rfc2898DeriveBytes.Pbkdf2(
            password, saltBytes, Iterations, HashAlgorithmName.SHA256, HashSize);
        // Constant-time compare — avoids timing side-channels.
        return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }
}
