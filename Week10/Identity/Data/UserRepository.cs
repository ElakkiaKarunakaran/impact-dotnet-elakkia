using System.Data;
using System.Diagnostics.CodeAnalysis;
using Identity.Models;
using Microsoft.Data.SqlClient;

namespace Identity.Data;

// Task 10.2 — ADO.NET repository for the security-critical Identity service.
// Every write is PARAMETERIZED and routed through a stored procedure.
// No EF on the login path: explicit SQL = explicit control.
public interface IUserRepository
{
    User? FindByUsername(string username);
    void  Insert(User user);
}

// Excluded from the coverage gate: these methods only do anything against a
// live SQL Server (ADO.NET, no mockable abstraction over SqlConnection).
// AuthControllerTests exercises every caller with IUserRepository mocked; this
// class itself is proven against the real IdentityDb + stored procs during the
// Task 10.8 docker-compose demo, not in the unit suite.
[ExcludeFromCodeCoverage]
public class UserRepository : IUserRepository
{
    private readonly string _conn;

    public UserRepository(IConfiguration cfg)
        => _conn = cfg.GetConnectionString("IdentityDb")
                 ?? throw new InvalidOperationException("IdentityDb connection missing");

    public User? FindByUsername(string username)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd  = new SqlCommand("usp_FindUserByUsername", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Username", username);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new User
        {
            Id           = reader.GetInt32(reader.GetOrdinal("Id")),
            Username     = reader.GetString(reader.GetOrdinal("Username")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
            Role         = reader.GetString(reader.GetOrdinal("Role")),
        };
    }

    public void Insert(User user)
    {
        using var conn = new SqlConnection(_conn);
        using var cmd  = new SqlCommand("usp_InsertUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Username",     user.Username);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
        cmd.Parameters.AddWithValue("@Role",         user.Role);
        conn.Open();
        user.Id = Convert.ToInt32(cmd.ExecuteScalar());
    }
}
