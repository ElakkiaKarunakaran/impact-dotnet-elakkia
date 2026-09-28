public class SingletonDemo
{
	public static void Run()
	{
		Console.WriteLine("=== Singleton from 5 Threads ===\n");

		var threads = new List<Thread>();

		for (int i = 1; i <= 5; i++)
		{
			int threadNum = i;

			var t = new Thread(() =>
			{
				Logger.Instance.Log($"Thread {threadNum} logging");
			});

			threads.Add(t);
		}

		// Start all 5 threads
		threads.ForEach(t => t.Start());

		// Wait for all 5 threads to finish
		threads.ForEach(t => t.Join());


		Console.WriteLine("\n=== Singleton from 5 Tasks ===\n");

		var tasks = new List<Task>();

		for (int i = 1; i <= 5; i++)
		{
			int taskNum = i;

			tasks.Add(Task.Run(() =>
			{
				Logger.Instance.Log($"Task {taskNum} logging");
			}));
		}

		// Wait for all 5 tasks to finish
		Task.WaitAll(tasks.ToArray());


		Console.WriteLine("\n=== Proof — Same Logger ===");

		var logger1 = Logger.Instance;
		var logger2 = Logger.Instance;
		var logger3 = Logger.Instance;

		Console.WriteLine($"Instance 1 hash: {logger1.GetHashCode()}");
		Console.WriteLine($"Instance 2 hash: {logger2.GetHashCode()}");
		Console.WriteLine($"Instance 3 hash: {logger3.GetHashCode()}");
	}
}