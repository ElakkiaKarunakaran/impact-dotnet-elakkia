public class Student
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string RollNumber { get; set; }

	private int age;
	public int Age
	{
		get => age;
		set
		{
			if (value < 5 || value > 100)
				throw new ArgumentException("Age must be between 5 and 100");
			age = value;
		}
	}
}