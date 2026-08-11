public class TempFileManager : IDisposable
{
	private string filePath;
	private bool disposed = false;

	// Constructor — creates the temp file
	public TempFileManager()
	{
		filePath = Path.GetTempFileName();
		File.WriteAllText(filePath, "Temporary content");
		Console.WriteLine($"File created: {filePath}");
	}

	public bool FileExists() => File.Exists(filePath);

	public string ReadContent()
	{
		if (disposed)
			throw new ObjectDisposedException(nameof(TempFileManager));
		return File.ReadAllText(filePath);
	}

	// Called when using block ends OR manually called
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
		// GC.SuppressFinalize tells GC:
		// "Don't call the finalizer — Dispose() already cleaned up"
		// Without this, cleanup would run TWICE
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				// Delete the file
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
					Console.WriteLine($"File deleted: {filePath}");
				}
			}
			disposed = true;
		}
	}

	// Finalizer — backup if developer forgot to call Dispose()
	// GC calls this automatically but timing is unpredictable
	// That's why we have BOTH:
	// Dispose() = fast and reliable when using block ends
	// Finalizer = last resort backup safety net
	~TempFileManager()
	{
		Dispose(false);
		Console.WriteLine("Finalizer ran — Dispose was not called!");
	}
}