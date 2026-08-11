public class InsufficientFundsException : Exception
{
	public decimal DeficitAmount { get; }
	public InsufficientFundsException(decimal deficitAmount)
		: base($"Insufficient funds. You are short by {deficitAmount:C}.")
	{
		DeficitAmount = deficitAmount;
	}
}