using Microsoft.EntityFrameworkCore;

// Task 7.6 — EF Core Code First DbContext
// Run migrations:
//   dotnet ef migrations add Initial  (first run)
//   dotnet ef migrations add AddEnrolledOn  (after Task 7.8)
//   dotnet ef database update
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<User>    Users    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Student — unique email + roll number, Task 7.6 Fluent API
        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.RollNumber).IsRequired().HasMaxLength(50);
            e.Property(s => s.Email).HasMaxLength(200).IsRequired(false);
            e.Property(s => s.InternalNotes).IsRequired(false);
            e.HasIndex(s => s.Email).IsUnique();
            e.HasIndex(s => s.RollNumber).IsUnique();
        });

        // User — unique username
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
            e.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("Student");
            e.HasIndex(u => u.Username).IsUnique();
        });

        // Task 7.6 — seed data via HasData (deterministic IDs required)
        modelBuilder.Entity<Student>().HasData(
            new Student { Id=1, Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com", EnrolledOn=new DateTime(2024,1,15) },
            new Student { Id=2, Name="Ramya",   Age=22, RollNumber="R002", Email="ramya@email.com",   EnrolledOn=new DateTime(2024,1,15) },
            new Student { Id=3, Name="Arjun",   Age=23, RollNumber="R003", Email="arjun@email.com",   EnrolledOn=new DateTime(2024,1,16) }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id=1, Username="admin",   PasswordHash="hashed_admin_pw",   Role="Admin"   },
            new User { Id=2, Username="elakkia", PasswordHash="hashed_elakkia_pw", Role="Student" }
        );
    }
}
