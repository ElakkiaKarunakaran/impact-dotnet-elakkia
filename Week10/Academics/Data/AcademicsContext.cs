using Academics.Models;
using Microsoft.EntityFrameworkCore;

namespace Academics.Data;

// Task 10.3 — EF Code First DbContext.
// Run `dotnet ef migrations add Initial` then `dotnet ef database update`.
public class AcademicsContext : DbContext
{
    public AcademicsContext(DbContextOptions<AcademicsContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Course>  Courses  { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(e =>
        {
            e.HasIndex(s => s.RollNumber).IsUnique();
            e.HasIndex(s => s.Email).IsUnique();
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
        });

        // Task 10.3 — seed data (deterministic IDs required for HasData).
        modelBuilder.Entity<Student>().HasData(
            new Student { Id=1, Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com", EnrolledOn=new DateTime(2026,1,15) },
            new Student { Id=2, Name="Ramya",   Age=22, RollNumber="R002", Email="ramya@email.com",   EnrolledOn=new DateTime(2026,1,15) },
            new Student { Id=3, Name="Arjun",   Age=23, RollNumber="R003", Email="arjun@email.com",   EnrolledOn=new DateTime(2026,1,16) }
        );
        modelBuilder.Entity<Course>().HasData(
            new Course { Id=1, Code="CS101", Title="Intro to Programming",  Credits=4 },
            new Course { Id=2, Code="CS201", Title="Data Structures",       Credits=4 },
            new Course { Id=3, Code="CS301", Title="Distributed Systems",   Credits=3 }
        );
        modelBuilder.Entity<Teacher>().HasData(
            new Teacher { Id=1, Name="Dr. Priya Rao",  Department="Computer Science", Email="priya@portal.edu"  },
            new Teacher { Id=2, Name="Dr. Vikram Nair", Department="Mathematics",     Email="vikram@portal.edu" }
        );
    }
}
