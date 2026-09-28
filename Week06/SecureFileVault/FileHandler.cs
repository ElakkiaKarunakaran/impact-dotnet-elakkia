public class FileHandler
{
	// Write a file
	public static void WriteFile(string path, string content)
	{
		using var writer = new StreamWriter(path);
		writer.Write(content);
		Console.WriteLine($"Written to: {path}");
	}

	// Read a file
	public static string ReadFile(string path)
	{
		using var reader = new StreamReader(path);
		return reader.ReadToEnd();
	}

	// Append to a file
	public static void AppendFile(string path, string content)
	{
		using var writer = new StreamWriter(path, append: true);
		writer.WriteLine(content);
	}

	// Stream copy — no ReadAllBytes
	public static void CopyFile(string source, string dest)
	{
		using var input = new FileStream(source, FileMode.Open, FileAccess.Read);
		using var output = new FileStream(dest, FileMode.Create, FileAccess.Write);
		input.CopyTo(output);
		Console.WriteLine($"Copied {source} → {dest}");
	}

	// Chunked read — 4KB at a time, never loads whole file
	public static void ReadInChunks(string path)
	{
		const int CHUNK = 4096; // 4KB buffer
		byte[] buffer = new byte[CHUNK];
		long totalBytes = 0;
		int bytesRead;

		using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
		while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
		{
			totalBytes += bytesRead;
			// Process chunk here — never whole file in memory
		}
		Console.WriteLine($"Read {totalBytes} bytes in {CHUNK}-byte chunks");
	}
}