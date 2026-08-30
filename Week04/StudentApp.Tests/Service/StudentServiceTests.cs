using Moq;
using Xunit;

public class StudentServiceTests
{
    // ── Helper — creates a fresh service with a mock repo ──
    // Every test gets its own fresh mock — no shared state
    private (StudentService service, Mock<IRepository<Student>> mockRepo)
        CreateService(List<Student>? existingStudents = null)
    {
        // Create a fake (mock) repository
        var mockRepo = new Mock<IRepository<Student>>();

        // Tell the mock: when GetAll() is called, return this list
        var students = existingStudents ?? new List<Student>();
        mockRepo.Setup(r => r.GetAll()).Returns(students);

        var service = new StudentService(mockRepo.Object);
        return (service, mockRepo);
    }

    // Helper — creates a valid student for reuse
    private Student ValidStudent(string roll = "R001") => new Student
    {
        Name       = "Elakkia",
        Age        = 21,
        RollNumber = roll,
        Email      = "elakkia@email.com"
    };

    // ════════════════════════════════════════
    // ADD STUDENT TESTS
    // ════════════════════════════════════════

    [Fact]
    public void AddStudent_ValidStudent_ReturnsSuccess()
    {
        // Arrange — set up the service with empty repo
        var (service, mockRepo) = CreateService();

        // Act — call the method
        var (success, message) = service.AddStudent(ValidStudent());

        // Assert — check results
        Assert.True(success);
        Assert.Equal("Student added successfully", message);

        // Verify repo.Add() was actually called once
        mockRepo.Verify(r => r.Add(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public void AddStudent_EmptyName_ReturnsFail()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var student = ValidStudent();
        student.Name = ""; // empty name

        // Act
        var (success, message) = service.AddStudent(student);

        // Assert
        Assert.False(success);
        Assert.Equal("Name cannot be empty", message);

        // Verify repo.Add() was NEVER called (rejected before reaching repo)
        mockRepo.Verify(r => r.Add(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public void AddStudent_InvalidAge_ReturnsFail()
    {
        // Arrange
        var (service, mockRepo) = CreateService();

        // Age validation happens in model — wrap in try/catch
        // OR test service-level age check
        var student = new Student
        {
            Name       = "Test",
            RollNumber = "R001",
            Email      = "test@email.com"
        };

        // Act — try to set invalid age
        // Service should catch this
        try { student.Age = 150; } // model throws
        catch { }

        var (success, message) = service.AddStudent(student);

        // Assert — age 0 (default) is invalid
        Assert.False(success);
        Assert.Contains("Age", message);
        mockRepo.Verify(r => r.Add(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public void AddStudent_DuplicateRollNumber_ReturnsFail()
    {
        // Arrange — repo already has a student with R001
        var existingStudents = new List<Student>
        {
            new Student { Id=1, Name="Existing", RollNumber="R001", Age=20 }
        };
        var (service, mockRepo) = CreateService(existingStudents);

        // Act — try to add another student with same roll number
        var (success, message) = service.AddStudent(ValidStudent("R001"));

        // Assert
        Assert.False(success);
        Assert.Contains("R001", message);
        Assert.Contains("already exists", message);
        mockRepo.Verify(r => r.Add(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public void AddStudent_NullName_ReturnsFail()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var student = ValidStudent();
        student.Name = null;

        // Act
        var (success, message) = service.AddStudent(student);

        // Assert
        Assert.False(success);
        Assert.Equal("Name cannot be empty", message);
    }

    [Fact]
    public void AddStudent_WhitespaceName_ReturnsFail()
    {
        // Arrange
        var (service, mockRepo) = CreateService();
        var student = ValidStudent();
        student.Name = "   "; // whitespace only

        // Act
        var (success, message) = service.AddStudent(student);

        // Assert
        Assert.False(success);
        Assert.Equal("Name cannot be empty", message);
    }

    // ════════════════════════════════════════
    // UPDATE STUDENT TESTS
    // ════════════════════════════════════════

    [Fact]
    public void UpdateStudent_ValidUpdate_ReturnsSuccess()
    {
        // Arrange — repo has student with ID 1
        var existing = new Student
            { Id=1, Name="Old Name", RollNumber="R001", Age=20 };

        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetAll())
            .Returns(new List<Student> { existing });
        mockRepo.Setup(r => r.GetById(1))
            .Returns(existing);

        var service = new StudentService(mockRepo.Object);
        var updated = new Student
            { Id=1, Name="New Name", RollNumber="R001", Age=21 };

        // Act
        var (success, message) = service.UpdateStudent(updated);

        // Assert
        Assert.True(success);
        Assert.Equal("Student updated successfully", message);
        mockRepo.Verify(r => r.Update(It.IsAny<Student>()), Times.Once);
    }

    [Fact]
    public void UpdateStudent_MissingId_ReturnsFail()
    {
        // Arrange — repo returns null for ID 999
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetAll()).Returns(new List<Student>());
        mockRepo.Setup(r => r.GetById(999)).Returns((Student)null);

        var service = new StudentService(mockRepo.Object);
        var student = new Student { Id=999, Name="Test", Age=20, RollNumber="R001" };

        // Act
        var (success, message) = service.UpdateStudent(student);

        // Assert
        Assert.False(success);
        Assert.Contains("not found", message);
        mockRepo.Verify(r => r.Update(It.IsAny<Student>()), Times.Never);
    }

    [Fact]
    public void UpdateStudent_DuplicateRollNumber_ReturnsFail()
    {
        // Arrange — two students exist, try to give student 1 the roll of student 2
        var s1 = new Student { Id=1, Name="S1", RollNumber="R001", Age=20 };
        var s2 = new Student { Id=2, Name="S2", RollNumber="R002", Age=21 };

        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetAll())
            .Returns(new List<Student> { s1, s2 });
        mockRepo.Setup(r => r.GetById(1)).Returns(s1);

        var service = new StudentService(mockRepo.Object);

        // Try to update s1 with R002 (belongs to s2)
        var updated = new Student { Id=1, Name="S1", RollNumber="R002", Age=20 };

        // Act
        var (success, message) = service.UpdateStudent(updated);

        // Assert
        Assert.False(success);
        Assert.Contains("already exists", message);
    }

    // ════════════════════════════════════════
    // DELETE STUDENT TESTS
    // ════════════════════════════════════════

    [Fact]
    public void DeleteStudent_ValidId_ReturnsSuccess()
    {
        // Arrange
        var existing = new Student { Id=1, Name="Elakkia", RollNumber="R001", Age=21 };
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetById(1)).Returns(existing);

        var service = new StudentService(mockRepo.Object);

        // Act
        var (success, message) = service.DeleteStudent(1);

        // Assert
        Assert.True(success);
        Assert.Equal("Student deleted successfully", message);
        mockRepo.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteStudent_MissingId_ReturnsFail()
    {
        // Arrange — repo returns null for ID 999
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetById(999)).Returns((Student)null);

        var service = new StudentService(mockRepo.Object);

        // Act
        var (success, message) = service.DeleteStudent(999);

        // Assert
        Assert.False(success);
        Assert.Contains("not found", message);
        mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
    }

    // ════════════════════════════════════════
    // TRANSACTION LOG TESTS
    // ════════════════════════════════════════

    [Fact]
    public void TransactionLog_RecordsAdd()
    {
        // Arrange
        var (service, _) = CreateService();

        // Act
        service.AddStudent(ValidStudent());
        var log = service.GetTransactionLog().ToList();

        // Assert
        Assert.Single(log); // exactly one entry
        Assert.Contains("ADDED", log[0]);
        Assert.Contains("Elakkia", log[0]);
    }

    [Fact]
    public void TransactionLog_RecordsUpdate()
    {
        // Arrange
        var existing = new Student
            { Id=1, Name="Elakkia", RollNumber="R001", Age=21 };
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetAll())
            .Returns(new List<Student> { existing });
        mockRepo.Setup(r => r.GetById(1)).Returns(existing);

        var service = new StudentService(mockRepo.Object);
        var updated = new Student
            { Id=1, Name="Elakkia Updated", RollNumber="R001", Age=22 };

        // Act
        service.UpdateStudent(updated);
        var log = service.GetTransactionLog().ToList();

        // Assert
        Assert.Single(log);
        Assert.Contains("UPDATED", log[0]);
    }

    [Fact]
    public void TransactionLog_RecordsDelete()
    {
        // Arrange
        var existing = new Student
            { Id=1, Name="Elakkia", RollNumber="R001", Age=21 };
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetById(1)).Returns(existing);

        var service = new StudentService(mockRepo.Object);

        // Act
        service.DeleteStudent(1);
        var log = service.GetTransactionLog().ToList();

        // Assert
        Assert.Single(log);
        Assert.Contains("DELETED", log[0]);
    }

    [Fact]
    public void TransactionLog_RecordsAllMutations()
    {
        // Arrange
        var existing = new Student
            { Id=1, Name="Elakkia", RollNumber="R001", Age=21 };
        var mockRepo = new Mock<IRepository<Student>>();
        mockRepo.Setup(r => r.GetAll())
            .Returns(new List<Student> { existing });
        mockRepo.Setup(r => r.GetById(1)).Returns(existing);

        var service = new StudentService(mockRepo.Object);

        // Act — add, update, delete
        service.AddStudent(ValidStudent("R002"));
        service.UpdateStudent(new Student
            { Id=1, Name="Updated", RollNumber="R001", Age=22 });
        service.DeleteStudent(1);

        var log = service.GetTransactionLog().ToList();

        // Assert — 3 entries in order
        Assert.Equal(3, log.Count);
        Assert.Contains("ADDED",   log[0]);
        Assert.Contains("UPDATED", log[1]);
        Assert.Contains("DELETED", log[2]);
    }

    [Fact]
    public void TransactionLog_FailedOperations_NotLogged()
    {
        // Arrange — failed add should NOT be logged
        var (service, _) = CreateService();
        var student = ValidStudent();
        student.Name = ""; // will fail

        // Act
        service.AddStudent(student);
        var log = service.GetTransactionLog().ToList();

        // Assert — nothing logged for failed operation
        Assert.Empty(log);
    }

    // ════════════════════════════════════════
    // MODEL VALIDATION TESTS
    // ════════════════════════════════════════

    [Fact]
    public void Student_ValidAge_SetsCorrectly()
    {
        var student = new Student();
        student.Age = 21;
        Assert.Equal(21, student.Age);
    }

    [Fact]
    public void Student_AgeTooLow_ThrowsException()
    {
        var student = new Student();
        Assert.Throws<ArgumentException>(() => student.Age = 4);
    }

    [Fact]
    public void Student_AgeTooHigh_ThrowsException()
    {
        var student = new Student();
        Assert.Throws<ArgumentException>(() => student.Age = 101);
    }

    [Fact]
    public void Student_AgeAtBoundary_SetsCorrectly()
    {
        var student = new Student();
        student.Age = 5;   // minimum
        Assert.Equal(5, student.Age);
        student.Age = 100; // maximum
        Assert.Equal(100, student.Age);
    }
}