using System.ComponentModel.DataAnnotations;

public class Student
{
	public int Id { get; set; }

	[Required]
	public string Name { get; set; }

	[Range(5, 100)]
	public int Age { get; set; }

	[Required]
	public string RollNumber { get; set; }

	[EmailAddress]
	public string Email { get; set; }

	public string InternalNotes { get; set; }
}