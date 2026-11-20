using System.Security.Cryptography;
using System.Text;

// In-memory IUserStore for when DataLayer = "InMemory"
// Seed passwords (SHA-256 hashed):
//   admin   → "admin123!"
//   elakkia → "elakkia123!"
//   teacher → "teacher123!"
public class InMemoryUserStore : IUserStore
{
    private static string Hash(string pw)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(pw)));

    private readonly List<User> _users = new()
    {
        new User { Id=1, Username="admin",   PasswordHash=Hash("admin123!"),   Role="Teacher" },
        new User { Id=2, Username="elakkia", PasswordHash=Hash("elakkia123!"), Role="Student" },
        new User { Id=3, Username="teacher", PasswordHash=Hash("teacher123!"), Role="Teacher" }
    };
    private int _nextId = 4;

    public User? FindByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username == username);

    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
    }
}
