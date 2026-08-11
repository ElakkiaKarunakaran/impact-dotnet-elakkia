

		BankAccount account = new BankAccount(1000);

		try
		{
			account.Withdraw(1200);
		}
		catch (InsufficientFundsException ex)
		{
			Console.WriteLine(ex.Message);
			Console.WriteLine($"Need ₹{ex.DeficitAmount} more.");
		}
		finally
		{
			Console.WriteLine("Withdrawal attempt logged.");
		}
	


// ── Task 3.2 ──────────────────────────────────────
Console.WriteLine("\n=== IDisposable Tests ===\n");

using (var manager = new TempFileManager())
{
	Console.WriteLine($"File exists inside using: {manager.FileExists()}");
	Console.WriteLine($"Content: {manager.ReadContent()}");
}
// Dispose() called automatically here

Console.WriteLine("After using block — file should be deleted");

// ── Task 3.3 ──────────────────────────────────────
Console.WriteLine("\n=== Async/Await Tests ===\n");

var asyncDemo = new AsyncDemo();
await asyncDemo.RunSequentialAsync();   // ~9000ms
await asyncDemo.RunConcurrentAsync();   // ~3000ms