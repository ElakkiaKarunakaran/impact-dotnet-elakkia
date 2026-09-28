using System.Security.Cryptography;
using System.Text;

public class HashComparisonDemo
{
	public static void Run()
	{
		byte[] hash1 =
			SHA256.HashData(
				Encoding.UTF8.GetBytes("Hello")
			);

		byte[] hash2 =
			SHA256.HashData(
				Encoding.UTF8.GetBytes("Hello")
			);

		byte[] hash3 =
			SHA256.HashData(
				Encoding.UTF8.GetBytes("World")
			);

		bool same1 =
			CryptographicOperations.FixedTimeEquals(
				hash1,
				hash2
			);

		bool same2 =
			CryptographicOperations.FixedTimeEquals(
				hash1,
				hash3
			);

		Console.WriteLine($"Same data: {same1}");
		Console.WriteLine($"Different data: {same2}");
	}
}