public partial class Employee
{
	public string Department { get; set; }

	// body of the partial method lives here
	partial void OnNameChanged(string newName)
	{
		Console.WriteLine($"Name was changed to: {newName}");
	}

	public void PrintDetails()
	{
		Console.WriteLine($"ID: {Id}, Name: {Name}, Dept: {Department}");
	}
}