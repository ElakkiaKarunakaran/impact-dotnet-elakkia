using System.Security.Cryptography;

public class LargeFileDemo
{
	// 1. Create a 100 MB file
	public static void CreateLargeFile(string path)
	{
		const int fileSize = 100 * 1024 * 1024;
		const int bufferSize = 4096;

		byte[] buffer = new byte[bufferSize];

		using FileStream output = new FileStream(
			path,
			FileMode.Create,
			FileAccess.Write
		);

		int remaining = fileSize;

		while (remaining > 0)
		{
			int bytesToWrite = Math.Min(
				buffer.Length,
				remaining
			);

			RandomNumberGenerator.Fill(
				buffer.AsSpan(0, bytesToWrite)
			);

			output.Write(
				buffer,
				0,
				bytesToWrite
			);

			remaining -= bytesToWrite;
		}

		Console.WriteLine("100 MB file created.");
	}


	// 2. Encrypt the file
	public static void EncryptFile(
		string inputPath,
		string outputPath,
		byte[] key,
		byte[] iv)
	{
		using FileStream input = new FileStream(
			inputPath,
			FileMode.Open,
			FileAccess.Read
		);

		using FileStream output = new FileStream(
			outputPath,
			FileMode.Create,
			FileAccess.Write
		);

		using Aes aes = Aes.Create();

		aes.Key = key;
		aes.IV = iv;

		using CryptoStream crypto = new CryptoStream(
			output,
			aes.CreateEncryptor(),
			CryptoStreamMode.Write
		);

		byte[] buffer = new byte[4096];

		int bytesRead;

		while ((bytesRead = input.Read(
			buffer,
			0,
			buffer.Length)) > 0)
		{
			crypto.Write(
				buffer,
				0,
				bytesRead
			);
		}

		// Important: CryptoStream must be disposed
		// so the final encrypted block/padding is written.
	}


	// 3. Decrypt the file
	public static void DecryptFile(
		string inputPath,
		string outputPath,
		byte[] key,
		byte[] iv)
	{
		using FileStream input = new FileStream(
			inputPath,
			FileMode.Open,
			FileAccess.Read
		);

		using FileStream output = new FileStream(
			outputPath,
			FileMode.Create,
			FileAccess.Write
		);

		using Aes aes = Aes.Create();

		aes.Key = key;
		aes.IV = iv;

		using CryptoStream crypto = new CryptoStream(
			input,
			aes.CreateDecryptor(),
			CryptoStreamMode.Read
		);

		byte[] buffer = new byte[4096];

		int bytesRead;

		while ((bytesRead = crypto.Read(
			buffer,
			0,
			buffer.Length)) > 0)
		{
			output.Write(
				buffer,
				0,
				bytesRead
			);
		}
	}


	// 4. Compare the original and decrypted files
	public static bool FilesAreIdentical(
		string file1,
		string file2)
	{
		using FileStream first = new FileStream(
			file1,
			FileMode.Open,
			FileAccess.Read
		);

		using FileStream second = new FileStream(
			file2,
			FileMode.Open,
			FileAccess.Read
		);

		// Different sizes = definitely different files
		if (first.Length != second.Length)
		{
			return false;
		}

		byte[] buffer1 = new byte[4096];
		byte[] buffer2 = new byte[4096];

		int bytesRead;

		while ((bytesRead = first.Read(
			buffer1,
			0,
			buffer1.Length)) > 0)
		{
			int secondBytesRead = second.Read(
				buffer2,
				0,
				buffer2.Length
			);

			if (bytesRead != secondBytesRead)
			{
				return false;
			}

			for (int i = 0; i < bytesRead; i++)
			{
				if (buffer1[i] != buffer2[i])
				{
					return false;
				}
			}
		}

		return true;
	}
}