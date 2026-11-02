using StudentApi.DTOs;

public static class StudentMapper
{
	// Entity → ReadDto (for responses)
	public static StudentReadDto ToReadDto(Student student)
	{
		return new StudentReadDto
		{
			Id = student.Id,
			Name = student.Name,
			Age = student.Age,
			RollNumber = student.RollNumber,
			Email = student.Email
			// InternalNotes deliberately NOT mapped
		};
	}

	// CreateDto → Entity (for requests)
	public static Student ToEntity(StudentCreateDto dto)
	{
		return new Student
		{
			Name = dto.Name,
			Age = dto.Age,
			RollNumber = dto.RollNumber,
			Email = dto.Email
		};
	}
}