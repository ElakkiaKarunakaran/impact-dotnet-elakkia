using Microsoft.Data.SqlClient;

// Task 7.5 — ADO.NET implementation of IUserStore
// Login now verifies against SQL Server instead of an in-memory list.
public class AdoNetUserRepository : IUserStore
{
    private readonly string _connectionString;

    public AdoNetUserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User? FindByUsername(string username)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username=@Username",
            conn);
        cmd.Parameters.AddWithValue("@Username", username);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new User
        {
            Id           = reader.GetInt32(0),
            Username     = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Role         = reader.GetString(3)
        };
    }

    public void Add(User user)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)",
            conn);
        cmd.Parameters.AddWithValue("@Username",     user.Username);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@Role",         user.Role);
        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
