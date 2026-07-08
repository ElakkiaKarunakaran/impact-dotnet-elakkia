public delegate double MathOperation(double a, double b);
public class Calculator
{

	public static double Add(double a, double b)
	{

		Console.WriteLine($"Sum:{a + b}");
		return a + b;
	}

	public static double Subtract(double a, double b)
	{

		Console.WriteLine($"Subtract:{a - b}");
		return a - b;
	}

	public static double Multiply(double a, double b)
	{

		Console.WriteLine($"Multiply:{a * b}");
		return a * b;
	}

	public static double Divide(double a, double b)
	{
		if (b == 0)
		{
			Console.WriteLine("Cannot divide by zero");
			return 0;
		}
		Console.WriteLine($"Divide:{a/b}");

		return a /b;
	}

	}