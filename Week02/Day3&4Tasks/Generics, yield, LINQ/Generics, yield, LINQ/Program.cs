	using System.Linq; // needed for Where, OrderBy, GroupBy, Select etc.
					   Console.WriteLine("Before foreach");

	foreach (int n in EvenNumbers.GetEvenNumbers(10))
	{
		Console.WriteLine($"Received: {n}");
	}

	Console.WriteLine("After foreach");

	var collection = new BookCollection();
	collection.AddBook(new Book("Zebra Tales", "Author C"));
	collection.AddBook(new Book("Apple Stories", "Author A"));
	collection.AddBook(new Book("Mango Days", "Author B"));

	Console.WriteLine("\n-- Books in alphabetical order --");
	foreach (Book b in collection) // uses GetEnumerator() automatically
	{
		Console.WriteLine($"{b.Title} by {b.Author}");
	}

	List<Employee> employees = new List<Employee>
	{
		new Employee("Elakkia",   "Engineering", 75000, new DateTime(2022, 6, 1)),
		new Employee("C",     "Engineering", 45000, new DateTime(2023, 1, 15)),
		new Employee("B", "HR",          60000, new DateTime(2021, 3, 10)),
		new Employee("A",   "Engineering", 90000, new DateTime(2020, 7, 20)),
		new Employee("M",     "HR",          42000, new DateTime(2023, 9, 5)),
		new Employee("O",     "Finance",     80000, new DateTime(2019, 11, 1)),
		new Employee("Z",     "Finance",     55000, new DateTime(2022, 4, 18)),
		new Employee("W",   "Engineering", 95000, new DateTime(2018, 2, 28)),
		new Employee("D",     "HR",          70000, new DateTime(2020, 8, 14)),
		new Employee("S",     "Finance",     48000, new DateTime(2023, 5, 30))
	};

	Console.WriteLine("=== Filter: Salary > 50000 ===");


	//Query syntax

	var Filtered_query_employees = from e in employees
								   where e.Salary > 50000
								   select e;

	foreach (var e in Filtered_query_employees)
	{
		Console.WriteLine($"Employees;{e} and salary {e.Salary}");
	}

	//Method syntax

	var Filtered_method_employees = employees.OrderByDescending(e => e.Salary);

	foreach (var e in employees) { Console.WriteLine($"Name:{e.Name},Salary:{e.Salary}"); }

	Console.WriteLine("\n=== Group by Department ===");

	// Query syntax
	var grouped_Q =
		from e in employees
		group e by e.Department into g
		select new
		{
			Department = g.Key,
			Count = g.Count(),
			AvgSalary = g.Average(e => e.Salary)
		};

	foreach (var g in grouped_Q)
		Console.WriteLine($"{g.Department}: {g.Count} employees, avg Rs.{g.AvgSalary:F0}");

	// Method syntax
	var grouped_M = employees
		.GroupBy(e => e.Department)
		.Select(g => new
		{
			Department = g.Key,
			Count = g.Count(),
			AvgSalary = g.Average(e => e.Salary)
		});

	foreach (var g in grouped_M)
		Console.WriteLine($"{g.Department}: {g.Count} employees, avg Rs.{g.AvgSalary:F0}");
