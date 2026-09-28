using System.Security.Cryptography;
using System.Text;

public class RsaDemo
{
	public static void Demonstrate()
	{
		using var rsa = RSA.Create(2048); // 2048-bit key pair

		string message = "Hello RSA"; // SHORT — RSA has size limit
		byte[] messageBytes = Encoding.UTF8.GetBytes(message);

		// Encrypt with PUBLIC key (anyone can encrypt)
		byte[] encrypted = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA256);

		// Decrypt with PRIVATE key (only key owner can decrypt)
		byte[] decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);

		Console.WriteLine($"Original:  {message}");
		Console.WriteLine($"Decrypted: {Encoding.UTF8.GetString(decrypted)}");

		/*
        WHICH KEY DOES WHAT:
        Public key  = encrypt data OR verify a signature
        Private key = decrypt data OR create a signature

        RSA SIZE LIMIT:
        2048-bit RSA can only encrypt ~245 bytes max
        Cannot encrypt large files directly

        HYBRID RSA+AES (what TLS uses):
        1. Generate random AES-256 key
        2. Encrypt the AES key with RSA (small, fits limit)
        3. Encrypt actual data with AES (fast, no size limit)
        4. Send: RSA-encrypted AES key + AES-encrypted data
        Receiver: decrypt AES key with RSA private key → decrypt data with AES
        */
	}
}