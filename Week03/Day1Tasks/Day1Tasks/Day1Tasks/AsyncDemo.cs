public class AsyncDemo
{
	// Simulates slow network call
	public async Task<string> FetchUserDataAsync(int userId)
	{
		Console.WriteLine($"  Starting fetch user {userId}...");

		await Task.Delay(3000); // non-blocking 3 second wait

		Console.WriteLine($"  Finished fetch user {userId}");
		return $"User{userId}_Data";
	}

	// Waits one at a time — SLOW (~9 seconds)
	public async Task RunSequentialAsync()
	{
		Console.WriteLine("=== Sequential ===");
		var sw = System.Diagnostics.Stopwatch.StartNew();

		var u1 = await FetchUserDataAsync(1); // wait 3s then continue
		var u2 = await FetchUserDataAsync(2); // wait another 3s
		var u3 = await FetchUserDataAsync(3); // wait another 3s

		sw.Stop();
		Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
		Console.WriteLine($"Results: {u1}, {u2}, {u3}\n");
	}

	// Starts all at once — FAST (~3 seconds)
	public async Task RunConcurrentAsync()
	{
		Console.WriteLine("=== Concurrent with WhenAll ===");
		var sw = System.Diagnostics.Stopwatch.StartNew();

		// Start all three WITHOUT awaiting immediately
		var t1 = FetchUserDataAsync(1); // starts now
		var t2 = FetchUserDataAsync(2); // starts now
		var t3 = FetchUserDataAsync(3); // starts now

		// Wait for ALL three to finish together
		var results = await Task.WhenAll(t1, t2, t3);

		sw.Stop();
		Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
		Console.WriteLine($"Results: {string.Join(", ", results)}\n");
	}
}