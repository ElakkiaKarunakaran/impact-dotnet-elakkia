using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
	private readonly IStudentService service;
	private readonly ILogger<StudentsController> logger;

	public StudentsController(
		IStudentService service,
		ILogger<StudentsController> logger)
	{
		this.service = service;
		this.logger = logger;
	}

	// GET: api/students
	[HttpGet]
	public IActionResult GetAll()
	{
		var students = service.GetAll()
		.Select(StudentMapper.ToReadDto); // map all to ReadDto
		return Ok(students);
	}

	[HttpGet("{id}")]
	public IActionResult GetById(int id)
	{
		var student = service.GetById(id);

		if (student == null)
		{
			logger.LogWarning(
				"Student with ID {Id} was not found.",
				id);

			return NotFound($"Student with ID {id} not found");
		}

		var dto = new StudentReadDto
		{
			Id = student.Id,
			Name = student.Name,
			Age = student.Age,
			RollNumber = student.RollNumber,
			Email = student.Email
		};

		return Ok(dto);
	}

	// POST: api/students
	[HttpPost]
	public IActionResult Add([FromBody] StudentCreateDto dto)
	{
		var student = StudentMapper.ToEntity(dto); // map dto to entity
		var (success, message) = service.AddStudent(student);
		if (!success) return BadRequest(message);
		return CreatedAtAction(nameof(GetById),
			new { id = student.Id },
			StudentMapper.ToReadDto(student));
	}

	// PUT: api/students/1
	[HttpPut("{id}")]
	public IActionResult Update(
		int id,
		[FromBody] StudentCreateDto dto)
	{
		// DTO → Entity
		var student = new Student
		{
			Id = id,
			Name = dto.Name,
			Age = dto.Age,
			RollNumber = dto.RollNumber,
			Email = dto.Email
		};

		var result = service.UpdateStudent(student);

		if (!result.Success)
			return NotFound(result.Message);

		return NoContent();
	}

	// DELETE: api/students/1
	[HttpDelete("{id}")]
	public IActionResult Delete(int id)
	{
		var result = service.DeleteStudent(id);

		if (!result.Success)
			return NotFound(result.Message);

		return NoContent();
	}


	// GET api/students/1/grade?type=gpa
	[HttpGet("{id}/grade")]
	public IActionResult GetGrade(int id, [FromQuery] string type = "percentage")
	{
		var student = service.GetById(id);
		if (student == null) return NotFound();

		// Factory picks strategy — controller never changes
		var strategy = GradeStrategyFactory.GetStrategy(type);
		var result = strategy.Calculate(85.0); // example score

		logger.LogInformation($"Grade calculated for student {id} using {type} strategy");
		return Ok(new { StudentId = id, Grade = result });
	}
	// GET api/students/search?name=ela
	[HttpGet("search")]
	public IActionResult Search([FromQuery] string name)
	{
		var results = service.Search(name)
			.Select(StudentMapper.ToReadDto);
		return Ok(results); // always 200, even if empty list
	}
}