using Microsoft.EntityFrameworkCore;
using Xunit;

// Task from spec: "For the EF Code First repository, add integration tests using
// the EF Core InMemory ... provider (CRUD round-trips)."
// These tests hit a real EF pipeline but use an isolated in-memory store per test.

public class EfRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly EfStudentRepository _sut;

    public EfRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolated per test
            .Options;
        _db = new AppDbContext(options);
        _sut = new EfStudentRepository(_db);
    }

    public void Dispose() => _db.Dispose();

    private static Student Sample(string roll = "R001") => new()
    {
        Name       = "Test Student",
        Age        = 20,
        RollNumber = roll,
        Email      = $"{roll}@test.com",
        EnrolledOn = new DateTime(2024, 1, 15)
    };

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Add_PersistsToDatabase()
    {
        var student = Sample();
        _sut.Add(student);

        Assert.True(student.Id > 0);
        Assert.Equal(1, _db.Students.Count());
    }

    // ── Read ─────────────────────────────────────────────────────────────────

    [Fact]
    public void GetAll_ReturnsAllPersistedStudents()
    {
        _sut.Add(Sample("R001"));
        _sut.Add(Sample("R002"));

        var all = _sut.GetAll().ToList();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsCorrectStudent()
    {
        var student = Sample("R001");
        _sut.Add(student);

        var found = _sut.GetById(student.Id);

        Assert.NotNull(found);
        Assert.Equal("R001", found!.RollNumber);
    }

    [Fact]
    public void GetById_NonExistentId_ReturnsNull()
    {
        var result = _sut.GetById(9999);
        Assert.Null(result);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_ChangesPersistedValues()
    {
        var student = Sample("R001");
        _sut.Add(student);

        student.Name = "Updated Name";
        student.Age  = 25;
        _sut.Update(student);

        var updated = _sut.GetById(student.Id);
        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal(25, updated.Age);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_RemovesStudentFromDatabase()
    {
        var student = Sample();
        _sut.Add(student);

        _sut.Delete(student.Id);

        Assert.Equal(0, _db.Students.Count());
    }

    [Fact]
    public void Delete_NonExistentId_DoesNotThrow()
    {
        var ex = Record.Exception(() => _sut.Delete(9999));
        Assert.Null(ex);
    }

    // ── EnrolledOn (Task 7.8 column) ─────────────────────────────────────────

    [Fact]
    public void EnrolledOn_IsPersistedAndRetrieved()
    {
        var date    = new DateTime(2024, 9, 1);
        var student = Sample("R001");
        student.EnrolledOn = date;
        _sut.Add(student);

        var found = _sut.GetById(student.Id);

        Assert.Equal(date, found!.EnrolledOn);
    }
}
