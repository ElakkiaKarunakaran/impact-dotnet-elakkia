public class StudentView
{
	// Shows the main menu
	public void ShowMenu()
	{
		Console.WriteLine("\n╔══════════════════════════════╗");
		Console.WriteLine("║   STUDENT MANAGEMENT SYSTEM  ║");
		Console.WriteLine("╚══════════════════════════════╝");
		Console.WriteLine("1. Add Student");
		Console.WriteLine("2. View All Students");
		Console.WriteLine("3. Find Student by ID");
		Console.WriteLine("4. Update Student");
		Console.WriteLine("5. Delete Student");
		Console.WriteLine("6. View Transaction Log");
		Console.WriteLine("0. Exit");
		Console.Write("\nEnter choice: ");
	}

	// Prints all students in a table
	public void PrintStudents(IEnumerable<Student> students)
	{
		var list = students.ToList();
		if (!list.Any())
		{
			Console.WriteLine("No students found.");
			return;
		}

		Console.WriteLine("\n{0,-5} {1,-20} {2,-5} {3,-10} {4,-25}",
			"ID", "Name", "Age", "Roll No", "Email");
		Console.WriteLine(new string('-', 70));

		foreach (var s in list)
		{
			Console.WriteLine("{0,-5} {1,-20} {2,-5} {3,-10} {4,-25}",
				s.Id, s.Name, s.Age, s.RollNumber, s.Email);
		}
	}

	// Prints a single student
	public void PrintStudent(Student student)
	{
		if (student == null)
		{
			Console.WriteLine("Student not found.");
			return;
		}
		Console.WriteLine($"\nID:   {student.Id}");
		Console.WriteLine($"Name: {student.Name}");
		Console.WriteLine($"Age:  {student.Age}");
		Console.WriteLine($"Roll: {student.RollNumber}");
		Console.WriteLine($"Email:{student.Email}");
	}

	// Asks user to input student details
	public Student PromptForStudent()
	{
		Console.Write("Name: ");
		string name = Console.ReadLine();

		Console.Write("Age: ");
		int.TryParse(Console.ReadLine(), out int age);

		Console.Write("Roll Number: ");
		string roll = Console.ReadLine();

		Console.Write("Email: ");
		string email = Console.ReadLine();

		return new Student
		{
			Name = name,
			Age = age,
			RollNumber = roll,
			Email = email
		};
	}

	// Asks for ID
	public int PromptForId(string prompt = "Enter ID: ")
	{
		Console.Write(prompt);
		int.TryParse(Console.ReadLine(), out int id);
		return id;
	}

	// Shows any message — success or error
	public void ShowMessage(string message)
	{
		Console.WriteLine($"\n→ {message}");
	}

	// Shows transaction log
	public void ShowTransactionLog(IEnumerable<string> logs)
	{
		var list = logs.ToList();
		Console.WriteLine("\n=== Transaction Log ===");
		if (!list.Any())
		{
			Console.WriteLine("No transactions yet.");
			return;
		}
		list.ForEach(Console.WriteLine);
	}
}