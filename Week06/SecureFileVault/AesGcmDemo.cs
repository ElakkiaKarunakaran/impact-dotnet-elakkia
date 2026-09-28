using System.Security.Cryptography;
using System.Text;

public class AesGcmDemo
{
	public static void Run()
	{
		string text = "Hello Elakkia";

		// Create AES key and nonce
		byte[] key = RandomNumberGenerator.GetBytes(32);
		byte[] nonce = RandomNumberGenerator.GetBytes(12);

		// Convert text to bytes
		byte[] plaintext = Encoding.UTF8.GetBytes(text);

		// Space for encrypted data and authentication tag
		byte[] ciphertext = new byte[plaintext.Length];
		byte[] tag = new byte[16];

		// ENCRYPT
		using (AesGcm aes = new AesGcm(key, 16))
		{
			aes.Encrypt(
				nonce,
				plaintext,
				ciphertext,
				tag
			);
		}

		Console.WriteLine("Encryption successful.");

		// DECRYPT original data
		byte[] decrypted = new byte[ciphertext.Length];

		using (AesGcm aes = new AesGcm(key, 16))
		{
			aes.Decrypt(
				nonce,
				ciphertext,
				tag,
				decrypted
			);
		}

		Console.WriteLine(
			$"Decrypted: {Encoding.UTF8.GetString(decrypted)}"
		);

		// TAMPER WITH ONE BYTE
		ciphertext[0] ^= 1;

		// Try decrypting tampered ciphertext
		try
		{
			byte[] tamperedDecrypted = new byte[ciphertext.Length];

			using (AesGcm aes = new AesGcm(key, 16))
			{
				aes.Decrypt(
					nonce,
					ciphertext,
					tag,
					tamperedDecrypted
				);
			}

			Console.WriteLine("Tampering was NOT detected.");
		}
		catch (CryptographicException)
		{
			Console.WriteLine("Tampering detected!");
		}
	}
}