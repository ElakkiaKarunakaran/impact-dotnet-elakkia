using System.Security.Cryptography;
using System.Text;

public class AesEncryption
{
	public static byte[] Encrypt(
		string text,
		byte[] key,
		out byte[] iv)
	{
		using Aes aes = Aes.Create();

		aes.KeySize = 256;
		aes.Key = key;
		aes.GenerateIV();

		iv = aes.IV;

		byte[] plaintext = Encoding.UTF8.GetBytes(text);

		using MemoryStream memory = new MemoryStream();

		using CryptoStream crypto = new CryptoStream(
			memory,
			aes.CreateEncryptor(),
			CryptoStreamMode.Write
		);

		crypto.Write(plaintext, 0, plaintext.Length);
		crypto.FlushFinalBlock();

		return memory.ToArray();
	}


	public static string Decrypt(
		byte[] ciphertext,
		byte[] key,
		byte[] iv)
	{
		using Aes aes = Aes.Create();

		aes.Key = key;
		aes.IV = iv;

		using MemoryStream memory = new MemoryStream();

		using CryptoStream crypto = new CryptoStream(
			memory,
			aes.CreateDecryptor(),
			CryptoStreamMode.Write
		);

		crypto.Write(ciphertext, 0, ciphertext.Length);
		crypto.FlushFinalBlock();

		byte[] plaintext = memory.ToArray();

		return Encoding.UTF8.GetString(plaintext);
	}
}