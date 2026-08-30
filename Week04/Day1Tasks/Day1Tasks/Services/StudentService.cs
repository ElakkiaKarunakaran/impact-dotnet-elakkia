public class StudentService : IStudentService
{
	private readonly IRepository<Student> repo;
	private readonly List<string> transactionLog = new();

	// Constructor injection — repo is passed in
	public StudentService(IRepository<Student> repo)
	{
		this.repo = repo;
	}

	public (bool Success, string Message) AddStudent(Student student)
	{
		// Rule 1 — name must not be empty
		if (string.IsNullOrWhiteSpace(student.Name))
			return (false, "Name cannot be empty");

		// Rule 2 — age must be valid (model validates but double-check)
		if (student.Age < 5 || student.Age > 100)
			return (false, "Age must be between 5 and 100");

		// Rule 3 — no duplicate roll numbers
		var existing = repo.GetAll()
			.FirstOrDefault(s => s.RollNumber == student.RollNumber);
		if (existing != null)
			return (false, $"Roll number {student.RollNumber} already exists");

		repo.Add(student);
		transactionLog.Add($"[{DateTime.Now:HH:mm:ss}] ADDED: {student.Name} (Roll: {student.RollNumber})");
		return (true, "Student added successfully");
	}

	public IEnumerable<Student> GetAll() => repo.GetAll();

	public Student GetById(int id) => repo.GetById(id);

	public (bool Success, string Message) UpdateStudent(Student student)
	{
		var existing = repo.GetById(student.Id);
		if (existing == null)
			return (false, $"Student with ID {student.Id} not found");

		if (string.IsNullOrWhiteSpace(student.Name))
			return (false, "Name cannot be empty");

		// Check duplicate roll number (excluding self)
		var duplicate = repo.GetAll()
			.FirstOrDefault(s => s.RollNumber == student.RollNumber
							  && s.Id != student.Id);
		if (duplicate != null)
			return (false, $"Roll number {student.RollNumber} already exists");

		repo.Update(student);
		transactionLog.Add($"[{DateTime.Now:HH:mm:ss}] UPDATED: {student.Name} (ID: {student.Id})");
		return (true, "Student updated successfully");
	}

	public (bool Success, string Message) DeleteStudent(int id)
	{
		var existing = repo.GetById(id);
		if (existing == null)
			return (false, $"Student with ID {id} not found");

		repo.Delete(id);
		transactionLog.Add($"[{DateTime.Now:HH:mm:ss}] DELETED: {existing.Name} (ID: {id})");
		return (true, "Student deleted successfully");
	}

	public IEnumerable<string> GetTransactionLog() => transactionLog;
}