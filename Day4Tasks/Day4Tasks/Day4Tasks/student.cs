public class Student
{
	// const - same for every Student, fixed forever, set right here
	public const int MinAge = 5;
	// readonly - can only be set in the constructor, but each Student can have a different value
	public readonly DateTime EnrolledOn;
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

	public string Name { get; set; }

	// Main constructor
	public Student(string name, int age)
	{
		Name = name;
		Age = age;  // goes through the validation above
		EnrolledOn = DateTime.Now;
	}

	// Chained constructor - calls the one above with a default age
	public Student(string name) : this(name, MinAge)
	{
	}

	// Overloaded CalculateGrade - same method name, different parameters
	public string CalculateGrade(int score)
	{
		return score >= 50 ? "Pass" : "Fail";
	}

	public string CalculateGrade(int score, bool isFinalExam)
	{
		if (isFinalExam && score < 60) return "Fail (final exam stricter)";
		return score >= 50 ? "Pass" : "Fail";
	}
}