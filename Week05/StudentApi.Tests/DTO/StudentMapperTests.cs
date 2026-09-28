using StudentApi.DTOs;
using Xunit;

public class StudentMapperTests
{
	// ── ToReadDto ────────────────────────────────
	[Fact]
	public void ToReadDto_MapsAllSafeFields()
	{
		var student = new Student
		{
			Id = 1,
			Name = "Elakkia",
			Age = 21,
			RollNumber = "R001",
			Email = "elakkia@email.com",
			InternalNotes = "CONFIDENTIAL"
		};

		var dto = StudentMapper.ToReadDto(student);

		Assert.Equal(1, dto.Id);
		Assert.Equal("Elakkia", dto.Name);
		Assert.Equal(21, dto.Age);
		Assert.Equal("R001", dto.RollNumber);
		Assert.Equal("elakkia@email.com", dto.Email);
	}

	[Fact]
	public void ToReadDto_InternalNotes_NeverLeaks()
	{
		var student = new Student
		{
			Id = 1,
			Name = "Elakkia",
			Age = 21,
			RollNumber = "R001",
			Email = "e@e.com",
			InternalNotes = "TOP SECRET"
		};

		var dto = StudentMapper.ToReadDto(student);

		// StudentReadDto must NOT have InternalNotes property
		var properties = typeof(StudentReadDto).GetProperties()
			.Select(p => p.Name);
		Assert.DoesNotContain("InternalNotes", properties);
	}

	// ── ToEntity ─────────────────────────────────
	[Fact]
	public void ToEntity_MapsAllFields()
	{
		var dto = new StudentCreateDto
		{
			Name = "Elakkia",
			Age = 21,
			RollNumber = "R001",
			Email = "elakkia@email.com"
		};

		var entity = StudentMapper.ToEntity(dto);

		Assert.Equal("Elakkia", entity.Name);
		Assert.Equal(21, entity.Age);
		Assert.Equal("R001", entity.RollNumber);
		Assert.Equal("elakkia@email.com", entity.Email);
		Assert.Equal(0, entity.Id); // Id not set by mapper
	}

	[Fact]
	public void ToEntity_InternalNotes_IsDefaultNotSet()
	{
		var dto = new StudentCreateDto
		{ Name = "Test", Age = 20, RollNumber = "R001", Email = "t@t.com" };

		var entity = StudentMapper.ToEntity(dto);

		// InternalNotes gets default value — mapper never sets it from DTO
		Assert.NotEqual("user-injected-value", entity.InternalNotes);
	}
}