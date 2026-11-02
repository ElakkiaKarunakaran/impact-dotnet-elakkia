// In-memory IUserStore for when DataLayer = "InMemory"
public class InMemoryUserStore : IUserStore
{
    private readonly List<User> _users = new()
    {
        new User { Id=1, Username="admin",   PasswordHash="hashed_admin_pw",   Role="Admin"   },
        new User { Id=2, Username="elakkia", PasswordHash="hashed_elakkia_pw", Role="Student" }
    };
    private int _nextId = 3;

    public User? FindByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username == username);

    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
    }
}
