using Microsoft.Data.SqlClient;

// Task 7.2 + 7.3 + 7.4 — ADO.NET implementation of IRepository<Student>
// All writes use parameterized commands (never string-concatenated SQL).
// GetById and Insert delegate to stored procedures (Task 7.4).
public class AdoNetStudentRepository : IRepository<Student>
{
    private readonly string _connectionString;

    public AdoNetStudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Task 7.2 — GetAll via SqlDataReader
    public IEnumerable<Student> GetAll()
    {
        var students = new List<Student>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            "SELECT Id, Name, Age, RollNumber, Email, InternalNotes, EnrolledOn FROM Students",
            conn);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            students.Add(Map(reader));
        return students;
    }

    // Task 7.2 + 7.4 — GetById via stored procedure
    public Student? GetById(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("usp_GetStudentById", conn)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    // Task 7.3 + 7.4 — Insert via stored procedure; injection-safe
    public void Add(Student student)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("usp_InsertStudent", conn)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Name",          student.Name);
        cmd.Parameters.AddWithValue("@Age",           student.Age);
        cmd.Parameters.AddWithValue("@RollNumber",    student.RollNumber);
        cmd.Parameters.AddWithValue("@Email",         (object?)student.Email         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InternalNotes", (object?)student.InternalNotes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EnrolledOn",    (object?)student.EnrolledOn    ?? DBNull.Value);
        conn.Open();
        var newId = cmd.ExecuteScalar();
        student.Id = Convert.ToInt32(newId);
    }

    // Task 7.3 — Update via parameterized command
    public void Update(Student student)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            @"UPDATE Students
              SET Name=@Name, Age=@Age, RollNumber=@RollNumber,
                  Email=@Email, InternalNotes=@InternalNotes, EnrolledOn=@EnrolledOn
              WHERE Id=@Id",
            conn);
        cmd.Parameters.AddWithValue("@Id",            student.Id);
        cmd.Parameters.AddWithValue("@Name",          student.Name);
        cmd.Parameters.AddWithValue("@Age",           student.Age);
        cmd.Parameters.AddWithValue("@RollNumber",    student.RollNumber);
        cmd.Parameters.AddWithValue("@Email",         (object?)student.Email         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InternalNotes", (object?)student.InternalNotes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EnrolledOn",    (object?)student.EnrolledOn    ?? DBNull.Value);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    // Task 7.3 — Delete via parameterized command
    public void Delete(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("DELETE FROM Students WHERE Id=@Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    private static Student Map(SqlDataReader r) => new Student
    {
        Id            = r.GetInt32(r.GetOrdinal("Id")),
        Name          = r.GetString(r.GetOrdinal("Name")),
        Age           = r.GetInt32(r.GetOrdinal("Age")),
        RollNumber    = r.GetString(r.GetOrdinal("RollNumber")),
        Email         = r.IsDBNull(r.GetOrdinal("Email"))         ? null : r.GetString(r.GetOrdinal("Email")),
        InternalNotes = r.IsDBNull(r.GetOrdinal("InternalNotes")) ? null : r.GetString(r.GetOrdinal("InternalNotes")),
        EnrolledOn    = r.IsDBNull(r.GetOrdinal("EnrolledOn"))    ? null : r.GetDateTime(r.GetOrdinal("EnrolledOn")),
    };
}
