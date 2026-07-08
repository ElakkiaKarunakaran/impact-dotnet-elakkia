public class Logger
{
	public virtual void Log(string message)
	{
		Console.WriteLine($"[Logger] {message}");
	}
}

public class FileLogger : Logger
{
	// "new" = hiding, NOT overriding
	public new void Log(string message)
	{
		Console.WriteLine($"[FileLogger] Writing to file: {message}");
	}
}