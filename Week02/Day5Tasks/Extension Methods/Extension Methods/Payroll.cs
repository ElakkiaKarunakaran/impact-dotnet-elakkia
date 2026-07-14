public abstract class PayrollEmployee
{
	public string Name { get; set; }
	public string Department { get; set; }

	public PayrollEmployee(string name, string dept)
	{
		Name = name;
		Department = dept;
	}

	public abstract double CalculateSalary();

	public virtual void PrintDetails()
	{
		Console.WriteLine($"{Name} ({Department}): Rs.{CalculateSalary():F0}");
	}
}

public interface ITaxable
{
	double CalculateTax();
}

public class FullTimeEmployee : PayrollEmployee, ITaxable
{
	public double MonthlySalary { get; set; }

	public FullTimeEmployee(string name, string dept, double salary)
		: base(name, dept) => MonthlySalary = salary;

	public override double CalculateSalary() => MonthlySalary * 12;

	// ITaxable — only FullTime pays tax
	public double CalculateTax() => CalculateSalary() * 0.10;

	public override void PrintDetails()
	{
		base.PrintDetails();
		Console.WriteLine($"  Tax: Rs.{CalculateTax():F0}");
	}
}

public class PartTimeEmployee : PayrollEmployee
{
	public double HourlyRate { get; set; }
	public int HoursWorked { get; set; }

	public PartTimeEmployee(string name, string dept, double rate, int hours)
		: base(name, dept)
	{
		HourlyRate = rate;
		HoursWorked = hours;
	}

	public override double CalculateSalary() => HourlyRate * HoursWorked;
}

public class ContractEmployee : PayrollEmployee
{
	public double ContractAmount { get; set; }

	public ContractEmployee(string name, string dept, double amount)
		: base(name, dept) => ContractAmount = amount;

	public override double CalculateSalary() => ContractAmount;
}

