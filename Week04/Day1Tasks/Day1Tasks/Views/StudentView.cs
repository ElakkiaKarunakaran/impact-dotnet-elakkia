public class StudentView
{
	public void ShowMenu()
	{
		Console.WriteLine("Student Management System");
		Console.WriteLine("1. Add Student");
		Console.WriteLine("2. View All Students");
		Console.WriteLine("3. Update Student");
		Console.WriteLine("4. Delete Student");
		Console.WriteLine("5. View Transaction Log");
		Console.WriteLine("6. Exit");
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

	public Student PromptForStudent()
	{
		Console.Write("Enter Name: ");
		string name = Console.ReadLine();

		Console.Write("Enter Age: ");
		int.TryParse(Console.ReadLine(), out int age);

		Console.Write("Enter Roll Number: ");
		string rollNumber = Console.ReadLine();

		Console.Write("Enter Email: ");
		string email = Console.ReadLine();

		return new Student
		{
			Name = name,
			Age = age,
			RollNumber = rollNumber,
			Email = email
		};
	}

	public int PromptForId(string prompt = "Enter ID: ")
	{
		Console.Write(prompt);
		int.TryParse(Console.ReadLine(), out int id);
		return id;
	}
	public void ShowMessage(string message)
	{
		Console.WriteLine($"\n→ {message}");
	}

	public void PrintStudent(Student student)
	{
		if (student == null)
		{
			Console.WriteLine("Student not found.");
			return;
		}

		Console.WriteLine($"ID: {student.Id}");
		Console.WriteLine($"Name: {student.Name}");
		Console.WriteLine($"Age: {student.Age}");
		Console.WriteLine($"Roll Number: {student.RollNumber}");
		Console.WriteLine($"Email: {student.Email}");
	}

}