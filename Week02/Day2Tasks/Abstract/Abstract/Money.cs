public class Money
{
	public double Amount { get; set; }
	public string Currency { get; set; }

	public Money(double amount, string currency)
	{
		Amount = amount;
		Currency = currency;
	}

	// + operator
	public static Money operator +(Money a, Money b)
	{
		if (a.Currency != b.Currency)
			throw new InvalidOperationException(
				$"Cannot add {a.Currency} and {b.Currency} — currency mismatch.");
		return new Money(a.Amount + b.Amount, a.Currency);
	}

	// == operator
	public static bool operator ==(Money a, Money b)
	{
		return a.Currency == b.Currency && a.Amount == b.Amount;
	}

	// != operator (required whenever you define ==)
	public static bool operator !=(Money a, Money b) => !(a == b);

	// > operator
	public static bool operator >(Money a, Money b)
	{
		if (a.Currency != b.Currency)
			throw new InvalidOperationException("Cannot compare different currencies.");
		return a.Amount > b.Amount;
	}

	// < operator
	public static bool operator <(Money a, Money b)
	{
		if (a.Currency != b.Currency)
			throw new InvalidOperationException("Cannot compare different currencies.");
		return a.Amount < b.Amount;
	}

	public override string ToString() => $"{Currency} {Amount}";
}