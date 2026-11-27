using Microsoft.EntityFrameworkCore;

namespace Reporting.Data;

// Task 10.4 — EF DB First.
// These classes represent the shape of an EXISTING reporting DB, as if produced by:
//   Scaffold-DbContext "Server=...;Database=ReportingDb;..." Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data
// The DB is the source of truth — `sql/reporting-init.sql` provisions it.
// No migrations here; another team owns the schema.

public class ReportingContext : DbContext
{
    public ReportingContext(DbContextOptions<ReportingContext> options) : base(options) { }

    public virtual DbSet<Enrollment>        Enrollments        { get; set; } = null!;
    public virtual DbSet<EnrollmentSummary> EnrollmentSummary  { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CourseCode).HasMaxLength(10).IsRequired();
            entity.Property(e => e.EnrolledOn).HasColumnType("datetime2");
        });

        // DB view — EF DB First is comfortable here because the DB owns the shape.
        modelBuilder.Entity<EnrollmentSummary>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_EnrollmentSummary");
        });
    }
}

public class Enrollment
{
    public int      Id          { get; set; }
    public string   StudentName { get; set; } = string.Empty;
    public string   CourseCode  { get; set; } = string.Empty;
    public DateTime EnrolledOn  { get; set; }
}

public class EnrollmentSummary
{
    public string CourseCode     { get; set; } = string.Empty;
    public int    StudentCount   { get; set; }
}
