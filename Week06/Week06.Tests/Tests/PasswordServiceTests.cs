using System.Security.Cryptography;
using Xunit;

namespace Week06.Tests;

public class PasswordServiceTests
{
	[Fact]
	public void HashPassword_SamePasswordAndSalt_ProducesSameHash()
	{
		// Arrange
		string password = "teacher123";

		byte[] salt = new byte[16];
		RandomNumberGenerator.Fill(salt);

		// Act
		byte[] hash1 = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		byte[] hash2 = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		// Assert
		Assert.Equal(hash1, hash2);
	}


	[Fact]
	public void HashPassword_DifferentSalt_ProducesDifferentHash()
	{
		// Arrange
		string password = "teacher123";

		byte[] salt1 = RandomNumberGenerator.GetBytes(16);
		byte[] salt2 = RandomNumberGenerator.GetBytes(16);

		// Act
		byte[] hash1 = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt1,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		byte[] hash2 = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt2,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		// Assert
		Assert.NotEqual(hash1, hash2);
	}


	[Fact]
	public void VerifyPassword_CorrectPassword_ReturnsTrue()
	{
		// Arrange
		string password = "teacher123";

		byte[] salt = RandomNumberGenerator.GetBytes(16);

		byte[] storedHash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		// Act
		byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		bool result = CryptographicOperations.FixedTimeEquals(
			storedHash,
			enteredHash);

		// Assert
		Assert.True(result);
	}


	[Fact]
	public void VerifyPassword_WrongPassword_ReturnsFalse()
	{
		// Arrange
		string correctPassword = "teacher123";
		string wrongPassword = "wrong123";

		byte[] salt = RandomNumberGenerator.GetBytes(16);

		byte[] storedHash = Rfc2898DeriveBytes.Pbkdf2(
			correctPassword,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		// Act
		byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
			wrongPassword,
			salt,
			100_000,
			HashAlgorithmName.SHA256,
			32);

		bool result = CryptographicOperations.FixedTimeEquals(
			storedHash,
			enteredHash);

		// Assert
		Assert.False(result);
	}
}