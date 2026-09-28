
using Xunit;

namespace StudentApi.Tests;

public class StudentServiceTests
{
	private StudentService CreateService()
	{
		var repository = new InMemoryRepository<Student>(
			s => s.Id,
			(s, id) => s.Id = id);

		repository.Seed(new List<Student>
		{
			new Student
			{
				Name = "Elakkia",
				Age = 21,
				RollNumber = "R001",
				Email = "elakkia@email.com"
			},
			new Student
			{
				Name = "Ramya",
				Age = 22,
				RollNumber = "R002",
				Email = "ramya@email.com"
			},
			new Student
			{
				Name = "Arjun",
				Age = 23,
				RollNumber = "R003",
				Email = "arjun@email.com"
			}
		});

		return new StudentService(repository);
	}

	// --------------------------------------------------
	// GET ALL
	// --------------------------------------------------

	[Fact]
	public void GetAll_ShouldReturnAllStudents()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.GetAll();

		// Assert
		Assert.Equal(3, result.Count());
	}

	// --------------------------------------------------
	// GET BY ID - FOUND
	// --------------------------------------------------

	[Fact]
	public void GetById_ShouldReturnStudent_WhenStudentExists()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.GetById(1);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Elakkia", result.Name);
	}

	// --------------------------------------------------
	// GET BY ID - NOT FOUND
	// --------------------------------------------------

	[Fact]
	public void GetById_ShouldReturnNull_WhenStudentDoesNotExist()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.GetById(999);

		// Assert
		Assert.Null(result);
	}

	// --------------------------------------------------
	// ADD STUDENT - SUCCESS
	// --------------------------------------------------

	[Fact]
	public void AddStudent_ShouldAddStudentSuccessfully()
	{
		// Arrange
		var service = CreateService();

		var student = new Student
		{
			Name = "Kavin",
			Age = 24,
			RollNumber = "R004",
			Email = "kavin@email.com"
		};

		// Act
		var result = service.AddStudent(student);

		// Assert
		Assert.True(result.Success);
		Assert.Equal("Student added successfully", result.Message);

		var students = service.GetAll();

		Assert.Equal(4, students.Count());
	}

	// --------------------------------------------------
	// UPDATE STUDENT - SUCCESS
	// --------------------------------------------------

	[Fact]
	public void UpdateStudent_ShouldUpdateStudent_WhenStudentExists()
	{
		// Arrange
		var service = CreateService();

		var student = new Student
		{
			Id = 1,
			Name = "Elakkia Updated",
			Age = 22,
			RollNumber = "R001",
			Email = "updated@email.com"
		};

		// Act
		var result = service.UpdateStudent(student);

		// Assert
		Assert.True(result.Success);

		var updatedStudent = service.GetById(1);

		Assert.NotNull(updatedStudent);
		Assert.Equal("Elakkia Updated", updatedStudent.Name);
		Assert.Equal(22, updatedStudent.Age);
		Assert.Equal("updated@email.com", updatedStudent.Email);
	}

	// --------------------------------------------------
	// UPDATE STUDENT - NOT FOUND
	// --------------------------------------------------

	[Fact]
	public void UpdateStudent_ShouldFail_WhenStudentDoesNotExist()
	{
		// Arrange
		var service = CreateService();

		var student = new Student
		{
			Id = 999,
			Name = "Unknown",
			Age = 25,
			RollNumber = "R999",
			Email = "unknown@email.com"
		};

		// Act
		var result = service.UpdateStudent(student);

		// Assert
		Assert.False(result.Success);
	}

	// --------------------------------------------------
	// DELETE STUDENT - SUCCESS
	// --------------------------------------------------

	[Fact]
	public void DeleteStudent_ShouldDeleteStudent_WhenStudentExists()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.DeleteStudent(1);

		// Assert
		Assert.True(result.Success);

		var student = service.GetById(1);

		Assert.Null(student);
	}

	// --------------------------------------------------
	// DELETE STUDENT - NOT FOUND
	// --------------------------------------------------

	[Fact]
	public void DeleteStudent_ShouldFail_WhenStudentDoesNotExist()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.DeleteStudent(999);

		// Assert
		Assert.False(result.Success);
	}

	// --------------------------------------------------
	// SEARCH - MATCH
	// --------------------------------------------------

	[Fact]
	public void Search_ShouldReturnMatchingStudents()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.Search("elakkia");

		// Assert
		Assert.Single(result);
		Assert.Equal("Elakkia", result.First().Name);
	}

	// --------------------------------------------------
	// SEARCH - CASE INSENSITIVE
	// --------------------------------------------------

	[Fact]
	public void Search_ShouldBeCaseInsensitive()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.Search("ELAKKIA");

		// Assert
		Assert.Single(result);
		Assert.Equal("Elakkia", result.First().Name);
	}

	// --------------------------------------------------
	// SEARCH - NO MATCH
	// --------------------------------------------------

	[Fact]
	public void Search_ShouldReturnEmpty_WhenNoStudentMatches()
	{
		// Arrange
		var service = CreateService();

		// Act
		var result = service.Search("XYZ");

		// Assert
		Assert.Empty(result);
	}
}