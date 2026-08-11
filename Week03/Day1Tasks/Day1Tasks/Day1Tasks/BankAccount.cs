public class BankAccount
{
	private decimal balance;

	public BankAccount(decimal openingBalance)
	{
		balance = openingBalance;
	}

	public void Withdraw(decimal amount)
	{
		if (amount > balance)
		{
			decimal deficit = amount - balance;

			throw new InsufficientFundsException(deficit);
		}

		balance -= amount;

		Console.WriteLine($"Withdrawal successful. Remaining Balance = {balance}");
	}
}