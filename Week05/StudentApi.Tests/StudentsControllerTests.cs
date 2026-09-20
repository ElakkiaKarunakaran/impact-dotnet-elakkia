using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentApi.Controllers;
using StudentApi.Models;
using StudentApi.Services;
using Xunit;

namespace StudentApi.Tests;

public class StudentsControllerTests
{
	private readonly Mock<IStudentService> serviceMock;
	private readonly StudentsController controller;

	public StudentsControllerTests()
	{
		serviceMock = new Mock<IStudentService>();
		controller = new StudentsController(serviceMock.Object);
	}

	// --------------------------------------------------
	// GET ALL - 200
	// --------------------------------------------------

	[Fact]
	public void GetAll_ShouldReturn200()
	{
		// Arrange
		var students = new List<Student>
		{
			new Student { Id = 1, Name = "Elakkia" },
			new Student { Id = 2, Name = "Ramya" }
		};

		serviceMock
			.Setup(s => s.GetAll())
			.Returns(students);

		// Act
		var result = controller.GetAll();

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);

		Assert.Equal(200, okResult.StatusCode);
		Assert.Equal(students, okResult.Value);
	}

	// --------------------------------------------------
	// GET BY ID - 200
	// --------------------------------------------------

	[Fact]
	public void GetById_ShouldReturn200_WhenStudentExists()
	{
		// Arrange
		var student = new Student
		{
			Id = 1,
			Name = "Elakkia"
		};

		serviceMock
			.Setup(s => s.GetById(1))
			.Returns(student);

		// Act
		var result = controller.GetById(1);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);

		Assert.Equal(200, okResult.StatusCode);
		Assert.Equal(student, okResult.Value);
	}

	// --------------------------------------------------
	// GET BY ID - 404
	// --------------------------------------------------

	[Fact]
	public void GetById_ShouldReturn404_WhenStudentDoesNotExist()
	{
		// Arrange
		serviceMock
			.Setup(s => s.GetById(999))
			.Returns((Student?)null);

		// Act
		var result = controller.GetById(999);

		// Assert
		Assert.IsType<NotFoundObjectResult>(result);
	}

	// --------------------------------------------------
	// POST - 201
	// --------------------------------------------------

	[Fact]
	public void Add_ShouldReturn201_WhenStudentIsAdded()
	{
		// Arrange
		var student = new Student
		{
			Id = 4,
			Name = "Kavin"
		};

		serviceMock
			.Setup(s => s.AddStudent(student))
			.Returns((true, "Student added successfully"));

		// Act
		var result = controller.Add(student);

		// Assert
		var createdResult =
			Assert.IsType<CreatedAtActionResult>(result);

		Assert.Equal(201, createdResult.StatusCode);
		Assert.Equal(student, createdResult.Value);
	}

	// --------------------------------------------------
	// POST - 400
	// --------------------------------------------------

	[Fact]
	public void Add_ShouldReturn400_WhenServiceFails()
	{
		// Arrange
		var student = new Student
		{
			Name = "Duplicate"
		};

		serviceMock
			.Setup(s => s.AddStudent(student))
			.Returns((false, "Student already exists"));

		// Act
		var result = controller.Add(student);

		// Assert
		var badRequest =
			Assert.IsType<BadRequestObjectResult>(result);

		Assert.Equal(400, badRequest.StatusCode);
	}

	// --------------------------------------------------
	// PUT - 204
	// --------------------------------------------------

	[Fact]
	public void Update_ShouldReturn204_WhenStudentExists()
	{
		// Arrange
		var student = new Student
		{
			Name = "Elakkia Updated"
		};

		serviceMock
			.Setup(s => s.UpdateStudent(It.IsAny<Student>()))
			.Returns((true, "Student updated successfully"));

		// Act
		var result = controller.Update(1, student);

		// Assert
		Assert.IsType<NoContentResult>(result);
	}

	// --------------------------------------------------
	// PUT - 404
	// --------------------------------------------------

	[Fact]
	public void Update_ShouldReturn404_WhenStudentDoesNotExist()
	{
		// Arrange
		var student = new Student
		{
			Name = "Unknown"
		};

		serviceMock
			.Setup(s => s.UpdateStudent(It.IsAny<Student>()))
			.Returns((false, "Student not found"));

		// Act
		var result = controller.Update(999, student);

		// Assert
		Assert.IsType<NotFoundObjectResult>(result);
	}

	// --------------------------------------------------
	// DELETE - 204
	// --------------------------------------------------

	[Fact]
	public void Delete_ShouldReturn204_WhenStudentExists()
	{
		// Arrange
		serviceMock
			.Setup(s => s.DeleteStudent(1))
			.Returns((true, "Student deleted successfully"));

		// Act
		var result = controller.Delete(1);

		// Assert
		Assert.IsType<NoContentResult>(result);
	}

	// --------------------------------------------------
	// DELETE - 404
	// --------------------------------------------------

	[Fact]
	public void Delete_ShouldReturn404_WhenStudentDoesNotExist()
	{
		// Arrange
		serviceMock
			.Setup(s => s.DeleteStudent(999))
			.Returns((false, "Student not found"));

		// Act
		var result = controller.Delete(999);

		// Assert
		Assert.IsType<NotFoundObjectResult>(result);
	}
}