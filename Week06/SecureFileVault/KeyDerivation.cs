using System.Security.Cryptography;

public class KeyDerivation
{
	public static byte[] DeriveKey(
		string password,
		byte[] salt)
	{
		const int iterations = 100_000;

		using var pbkdf2 = new Rfc2898DeriveBytes(password,salt,iterations,HashAlgorithmName.SHA256);

		return pbkdf2.GetBytes(32);
	}
}