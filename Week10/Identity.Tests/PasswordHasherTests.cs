using Identity.Services;
using Xunit;

namespace Identity.Tests;

// Task 10.2 — PBKDF2 correctness tests (unit, no HTTP pipeline).
public class PasswordHasherTests
{
    [Fact]
    public void Hash_ThenVerify_SamePassword_Succeeds()
    {
        var (hash, salt) = PasswordHasher.Hash("correct-horse-battery-staple");
        Assert.True(PasswordHasher.Verify("correct-horse-battery-staple", hash, salt));
    }

    [Fact]
    public void Verify_WrongPassword_Fails()
    {
        var (hash, salt) = PasswordHasher.Hash("correct-horse-battery-staple");
        Assert.False(PasswordHasher.Verify("wrong-password", hash, salt));
    }

    [Fact]
    public void Hash_SamePasswordTwice_ProducesDifferentSalts()
    {
        var (hash1, salt1) = PasswordHasher.Hash("same-password");
        var (hash2, salt2) = PasswordHasher.Hash("same-password");

        Assert.NotEqual(salt1, salt2);
        Assert.NotEqual(hash1, hash2);  // different salt → different hash, even for the same password
    }

    [Fact]
    public void Verify_WithSwappedSalt_Fails()
    {
        var (hash1, _)     = PasswordHasher.Hash("password-one");
        var (_, otherSalt) = PasswordHasher.Hash("password-two");

        Assert.False(PasswordHasher.Verify("password-one", hash1, otherSalt));
    }
}
