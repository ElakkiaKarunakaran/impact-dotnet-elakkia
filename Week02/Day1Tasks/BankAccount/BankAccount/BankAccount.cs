public class BankAccount
{
	private double balance;
	private List<string> history = new List<string>();

	public BankAccount(double initialBalance)
	{
		if (initialBalance < 0)
			throw new ArgumentException("Initial balance cannot be negative.");
		balance = initialBalance;
		history.Add($"Account opened with Rs.{balance}");
	}

	public void Deposit(double amount)
	{
		if (amount <= 0)
		{
			Console.WriteLine("Deposit amount must be positive.");
			return;
		}
		balance += amount;
		history.Add($"Deposited Rs.{amount}. New balance: Rs.{balance}");
	}

	public void Withdraw(double amount)
	{
		if (amount <= 0)
		{
			Console.WriteLine("Withdrawal amount must be positive.");
			return;
		}
		if (amount > balance)
		{
			Console.WriteLine($"Overdraw rejected. Balance is only Rs.{balance}");
			return;
		}
		balance -= amount;
		history.Add($"Withdrew Rs.{amount}. New balance: Rs.{balance}");
	}

	public double GetBalance() => balance;

	public void PrintHistory()
	{
		Console.WriteLine("\n--- Transaction History ---");
		foreach (var entry in history)
			Console.WriteLine(entry);
	}
}