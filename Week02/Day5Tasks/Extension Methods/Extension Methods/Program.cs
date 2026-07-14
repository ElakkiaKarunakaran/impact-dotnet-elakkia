using System;
using System.Collections.Generic;
using System.Linq;

List<PayrollEmployee> staff = new List<PayrollEmployee>
{
	new FullTimeEmployee("Elakkia",   "Engineering", 75000),
	new FullTimeEmployee("K",   "Engineering", 90000),
	new PartTimeEmployee("R",     "HR",    500, 80),
	new PartTimeEmployee("Priya",     "HR",    450, 60),
	new ContractEmployee("Ar",     "Finance", 200000),
	new ContractEmployee("A", "Finance", 150000)
};

// Print details — polymorphism picks right method
Console.WriteLine("=== Employee Details ===");
foreach (var e in staff)
	e.PrintDetails();

// Total payroll
double total = staff.Sum(e => e.CalculateSalary());
Console.WriteLine($"\nTotal Payroll: Rs.{total:F0}");

// LINQ GroupBy department
Console.WriteLine("\n=== By Department ===");
var grouped = staff
	.GroupBy(e => e.Department)
	.Select(g => new
	{
		Department = g.Key,
		Count = g.Count(),
		TotalSalary = g.Sum(e => e.CalculateSalary())
	});

foreach (var g in grouped)
	Console.WriteLine($"{g.Department}: {g.Count} employees, total Rs.{g.TotalSalary:F0}");


public abstract class Animal
{
	public abstract void Speak();
	public void Sleep() => Console.WriteLine("Sleeping");
}
public class Cat : Animal
{
	public override void Speak() => Console.WriteLine("Meow");
}


