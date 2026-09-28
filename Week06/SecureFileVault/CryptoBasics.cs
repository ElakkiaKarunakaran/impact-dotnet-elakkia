using System.Security.Cryptography;
using System.Text;

public class CryptoBasics
{
	public static void DemonstrateThreeWays(string input)
	{
		Console.WriteLine($"Input: {input}\n");

		// 1. ENCODING — just representation, no secret, reversible by anyone
		string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
		Console.WriteLine($"Base64 encoded: {base64}");
		Console.WriteLine($"Base64 decoded: {decoded}");
		Console.WriteLine("→ Encoding: reversible, no key, not secret\n");

		// 2. HASHING — one-way, fixed length, no key
		byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
		string hash = Convert.ToHexString(hashBytes);
		Console.WriteLine($"SHA256 hash: {hash}");
		Console.WriteLine($"Hash length: always {hash.Length} chars");
		Console.WriteLine("→ Hashing: one-way, fixed length, can't reverse\n");

		// 3. ENCRYPTION — needs a key, reversible WITH key
		using var aes = Aes.Create();
		aes.GenerateKey();
		aes.GenerateIV();
		Console.WriteLine($"Encryption key: {Convert.ToBase64String(aes.Key)}");
		Console.WriteLine("→ Encryption: reversible ONLY with the key\n");

		/*
        ONE LINE DISTINCTION:
        Encoding   = representation change, no secret, anyone reverses it
        Hashing    = fingerprint, no key, one-way, fixed length
        Encryption = secret transformation, needs key to reverse

        WHEN EACH IS THE WRONG TOOL:
        ❌ Don't use encoding to "hide" data — Base64 is not encryption
        ❌ Don't hash passwords without salt — rainbow tables break it
        ❌ Don't use encryption when you just need to store a password
           (use hashing with PBKDF2 instead)
        */
	}
}