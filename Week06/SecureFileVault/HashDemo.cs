using System.Security.Cryptography;

public class HashDemo
{
	public static void Run()
	{
		string filePath = "students.txt";

		// SHA256
		byte[] sha256Hash =
			SHA256.HashData(File.ReadAllBytes(filePath));

		Console.WriteLine(
			$"SHA256: {Convert.ToHexString(sha256Hash)}"
		);

		// SHA512
		byte[] sha512Hash =
			SHA512.HashData(File.ReadAllBytes(filePath));

		Console.WriteLine(
			$"SHA512: {Convert.ToHexString(sha512Hash)}"
		);

		// HMACSHA256
		byte[] secretKey =
			RandomNumberGenerator.GetBytes(32);

		byte[] differentKey =
	RandomNumberGenerator.GetBytes(32);

		byte[] fileBytes =
			File.ReadAllBytes(filePath);

		using HMACSHA256 hmac =
			new HMACSHA256(differentKey);

		byte[] hmacValue =
			hmac.ComputeHash(fileBytes);

		Console.WriteLine(
			$"HMAC: {Convert.ToHexString(hmacValue)}"
		);
	}
}