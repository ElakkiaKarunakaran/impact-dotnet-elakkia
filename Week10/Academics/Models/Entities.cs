using System.ComponentModel.DataAnnotations;

namespace Academics.Models;

// Task 10.3 — core Academics domain: Student, Course, Teacher.
// Code-First: C# classes drive the schema via EF migrations.

public class Student
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(5, 100)]
    public int Age { get; set; }

    [Required, MaxLength(50)]
    public string RollNumber { get; set; } = string.Empty;

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
}

public class Course
{
    public int Id { get; set; }

    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;  // e.g. "CS101"

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 6)]
    public int Credits { get; set; }
}

public class Teacher
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }
}
