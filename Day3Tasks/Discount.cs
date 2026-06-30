namespace Day3Tasks
{
	public class Discount
	{
		public double ApplyDiscount(double? discount)
		{
			double discountApplied = discount ?? 0.05;
			return discountApplied;
		}
	}
}