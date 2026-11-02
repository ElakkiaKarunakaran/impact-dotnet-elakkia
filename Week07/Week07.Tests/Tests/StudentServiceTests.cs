using Moq;
using Xunit;

// Task from spec: "Unit-test the service against a MOCKED IRepository<>
// ... reaching ≥80% on the service."
// The service logic must not depend on which data layer is behind it.

public class StudentServiceTests
{
    private readonly Mock<IRepository<Student>> _repoMock;
    private readonly StudentService _sut;

    public StudentServiceTests()
    {
        _repoMock = new Mock<IRepository<Student>>();
        _sut = new StudentService(_repoMock.Object);
    }

    // ── AddStudent ───────────────────────────────────────────────────────────

    [Fact]
    public void AddStudent_ValidStudent_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>());

        var result = _sut.AddStudent(new Student
            { Name="TestUser", Age=20, RollNumber="T001", Email="t@email.com" });

        Assert.True(result.Success);
        Assert.Equal("Student added successfully", result.Message);
        _repoMock.Verify(r => r.Add(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public void AddStudent_EmptyName_ReturnsFail()
    {
        var result = _sut.AddStudent(new Student { Name="", Age=20, RollNumber="T001" });

        Assert.False(result.Success);
        Assert.Contains("Name", result.Message);
        _repoMock.Verify(r => r.Add(It.IsAny<Student>()), Times.Never);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(101)]
    public void AddStudent_InvalidAge_ReturnsFail(int age)
    {
        var result = _sut.AddStudent(new Student { Name="Alice", Age=age, RollNumber="T002" });

        Assert.False(result.Success);
        Assert.Contains("Age", result.Message);
    }

    [Fact]
    public void AddStudent_DuplicateRollNumber_ReturnsFail()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>
            { new() { Id=1, Name="Existing", Age=20, RollNumber="DUP" } });

        var result = _sut.AddStudent(new Student { Name="New", Age=22, RollNumber="DUP" });

        Assert.False(result.Success);
        Assert.Contains("DUP", result.Message);
    }

    // ── SQL injection safety (Task 7.3) ──────────────────────────────────────

    [Fact]
    public void AddStudent_InjectionString_StoredAsLiteralText()
    {
        // The service must not reject the injection string — it should pass it
        // to the repo, which parameterises the SQL so it is stored literally.
        var injectionName = "'; DROP TABLE Students;--";
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>());
        Student? captured = null;
        _repoMock.Setup(r => r.Add(It.IsAny<Student>()))
                 .Callback<Student>(s => captured = s);

        var result = _sut.AddStudent(new Student
            { Name=injectionName, Age=20, RollNumber="INJ01" });

        Assert.True(result.Success);
        Assert.Equal(injectionName, captured!.Name);
    }

    // ── GetAll / GetById ──────────────────────────────────────────────────────

    [Fact]
    public void GetAll_ReturnsAllStudents()
    {
        var students = new List<Student>
        {
            new() { Id=1, Name="A", Age=20, RollNumber="R1" },
            new() { Id=2, Name="B", Age=21, RollNumber="R2" }
        };
        _repoMock.Setup(r => r.GetAll()).Returns(students);

        var result = _sut.GetAll().ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsStudent()
    {
        var student = new Student { Id=1, Name="Alice", Age=20, RollNumber="R1" };
        _repoMock.Setup(r => r.GetById(1)).Returns(student);

        var result = _sut.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public void GetById_MissingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((Student?)null);

        var result = _sut.GetById(99);

        Assert.Null(result);
    }

    // ── UpdateStudent ─────────────────────────────────────────────────────────

    [Fact]
    public void UpdateStudent_ExistingStudent_ReturnsSuccess()
    {
        var existing = new Student { Id=1, Name="Old", Age=20, RollNumber="R1" };
        _repoMock.Setup(r => r.GetById(1)).Returns(existing);
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student> { existing });

        var result = _sut.UpdateStudent(
            new Student { Id=1, Name="New", Age=22, RollNumber="R1" });

        Assert.True(result.Success);
        _repoMock.Verify(r => r.Update(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public void UpdateStudent_NonExistentId_ReturnsFail()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((Student?)null);

        var result = _sut.UpdateStudent(
            new Student { Id=99, Name="Ghost", Age=20, RollNumber="R99" });

        Assert.False(result.Success);
        Assert.Contains("99", result.Message);
    }

    [Fact]
    public void UpdateStudent_DuplicateRollNumber_ReturnsFail()
    {
        var s1 = new Student { Id=1, Name="Alice", Age=20, RollNumber="R1" };
        var s2 = new Student { Id=2, Name="Bob",   Age=21, RollNumber="R2" };
        _repoMock.Setup(r => r.GetById(1)).Returns(s1);
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student> { s1, s2 });

        var result = _sut.UpdateStudent(
            new Student { Id=1, Name="Alice", Age=20, RollNumber="R2" });

        Assert.False(result.Success);
    }

    // ── DeleteStudent ─────────────────────────────────────────────────────────

    [Fact]
    public void DeleteStudent_ExistingId_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.GetById(1)).Returns(
            new Student { Id=1, Name="Alice", Age=20, RollNumber="R1" });

        var result = _sut.DeleteStudent(1);

        Assert.True(result.Success);
        _repoMock.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteStudent_NonExistentId_ReturnsFail()
    {
        _repoMock.Setup(r => r.GetById(99)).Returns((Student?)null);

        var result = _sut.DeleteStudent(99);

        Assert.False(result.Success);
    }

    // ── Search ────────────────────────────────────────────────────────────────

    [Fact]
    public void Search_MatchingName_ReturnsSubset()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>
        {
            new() { Id=1, Name="Elakkia", Age=21, RollNumber="R1" },
            new() { Id=2, Name="Ramya",   Age=22, RollNumber="R2" }
        });

        var result = _sut.Search("ela").ToList();

        Assert.Single(result);
        Assert.Equal("Elakkia", result[0].Name);
    }

    [Fact]
    public void Search_EmptyQuery_ReturnsAll()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>
        {
            new() { Id=1, Name="A", Age=20, RollNumber="R1" },
            new() { Id=2, Name="B", Age=21, RollNumber="R2" }
        });

        var result = _sut.Search("").ToList();

        Assert.Equal(2, result.Count);
    }

    // ── Transaction log ───────────────────────────────────────────────────────

    [Fact]
    public void GetTransactionLog_AfterAddAndDelete_ContainsBothEntries()
    {
        _repoMock.Setup(r => r.GetAll()).Returns(new List<Student>());
        _repoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(
            new Student { Id=1, Name="X", Age=20, RollNumber="R1" });

        _sut.AddStudent(new Student { Name="X", Age=20, RollNumber="R1" });
        _sut.DeleteStudent(1);

        var log = _sut.GetTransactionLog().ToList();
        Assert.Equal(2, log.Count);
        Assert.Contains("ADDED",   log[0]);
        Assert.Contains("DELETED", log[1]);
    }
}
