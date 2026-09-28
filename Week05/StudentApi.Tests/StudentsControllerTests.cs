using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StudentApi.Controllers;
using StudentApi.DTOs;
using Xunit;

public class StudentsControllerTests
{
	private Mock<IStudentService> MockService() => new Mock<IStudentService>();
	private Mock<ILogger<StudentsController>> MockLogger()
		=> new Mock<ILogger<StudentsController>>();

	// ── GET ALL ──────────────────────────────────
	[Fact]
	public void GetAll_Returns200WithList()
	{
		var svc = MockService();
		svc.Setup(s => s.GetAll()).Returns(new List<Student>
		{
			new Student { Id=1, Name="Elakkia", RollNumber="R001", Age=21 }
		});
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.GetAll() as OkObjectResult;

		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	// ── GET BY ID ────────────────────────────────
	[Fact]
	public void GetById_Found_Returns200()
	{
		var svc = MockService();
		svc.Setup(s => s.GetById(1))
		   .Returns(new Student
		   {
			   Id = 1,
			   Name = "Elakkia",
			   RollNumber = "R001",
			   Age = 21
		   });
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.GetById(1) as OkObjectResult;

		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public void GetById_NotFound_Returns404()
	{
		var svc = MockService();
		svc.Setup(s => s.GetById(999)).Returns((Student)null);
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.GetById(999);

		Assert.IsType<NotFoundObjectResult>(result);
	}

	// ── POST ────────────────────────────────────
	[Fact]
	public void Add_ValidStudent_Returns201()
	{
		var svc = MockService();
		svc.Setup(s => s.AddStudent(It.IsAny<Student>()))
		   .Returns((true, "Student added successfully"));
		svc.Setup(s => s.GetById(It.IsAny<int>()))
		   .Returns(new Student
		   {
			   Id = 1,
			   Name = "Elakkia",
			   RollNumber = "R001",
			   Age = 21
		   });
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var dto = new StudentCreateDto
		{
			Name = "Elakkia",
			Age = 21,
			RollNumber = "R001",
			Email = "e@e.com"
		};
		var result = controller.Add(dto);

		Assert.IsType<CreatedAtActionResult>(result);
		var created = result as CreatedAtActionResult;
		Assert.Equal(201, created.StatusCode);
	}

	[Fact]
	public void Add_InvalidStudent_Returns400()
	{
		var svc = MockService();
		svc.Setup(s => s.AddStudent(It.IsAny<Student>()))
		   .Returns((false, "Roll number already exists"));
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var dto = new StudentCreateDto
		{
			Name = "Test",
			Age = 21,
			RollNumber = "R001",
			Email = "t@t.com"
		};
		var result = controller.Add(dto);

		Assert.IsType<BadRequestObjectResult>(result);
		var bad = result as BadRequestObjectResult;
		Assert.Equal(400, bad.StatusCode);
	}

	// ── PUT ──────────────────────────────────────
	[Fact]
	public void Update_ValidId_Returns204()
	{
		var svc = MockService();
		svc.Setup(s => s.UpdateStudent(It.IsAny<Student>()))
		   .Returns((true, "Student updated successfully"));
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var dto = new StudentCreateDto
		{
			Name = "Updated",
			Age = 22,
			RollNumber = "R001",
			Email = "u@u.com"
		};
		var result = controller.Update(1, dto);

		Assert.IsType<NoContentResult>(result);
		var noContent = result as NoContentResult;
		Assert.Equal(204, noContent.StatusCode);
	}

	[Fact]
	public void Update_MissingId_Returns404()
	{
		var svc = MockService();
		svc.Setup(s => s.UpdateStudent(It.IsAny<Student>()))
		   .Returns((false, "Student with ID 999 not found"));
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var dto = new StudentCreateDto
		{ Name = "X", Age = 20, RollNumber = "R999", Email = "x@x.com" };
		var result = controller.Update(999, dto);

		Assert.IsType<NotFoundObjectResult>(result);
	}

	// ── DELETE ───────────────────────────────────
	[Fact]
	public void Delete_ValidId_Returns204()
	{
		var svc = MockService();
		svc.Setup(s => s.DeleteStudent(1))
		   .Returns((true, "Student deleted successfully"));
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.Delete(1);

		Assert.IsType<NoContentResult>(result);
	}

	[Fact]
	public void Delete_MissingId_Returns404()
	{
		var svc = MockService();
		svc.Setup(s => s.DeleteStudent(999))
		   .Returns((false, "Student with ID 999 not found"));
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.Delete(999);

		Assert.IsType<NotFoundObjectResult>(result);
	}

	// ── SEARCH ───────────────────────────────────
	[Fact]
	public void Search_Returns200WithMatches()
	{
		var svc = MockService();
		svc.Setup(s => s.Search("ela")).Returns(new List<Student>
		{
			new Student { Id=1, Name="Elakkia",
						  RollNumber="R001", Age=21 }
		});
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.Search("ela") as OkObjectResult;

		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public void Search_NoMatch_Returns200EmptyList()
	{
		var svc = MockService();
		svc.Setup(s => s.Search("xyz"))
		   .Returns(new List<Student>());
		var controller = new StudentsController(svc.Object, MockLogger().Object);

		var result = controller.Search("xyz") as OkObjectResult;

		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}
}