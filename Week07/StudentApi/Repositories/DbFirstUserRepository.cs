using StudentApi.Data.DbFirst;

// DB First implementation of IUserStore
public class DbFirstUserRepository : IUserStore
{
    private readonly DbFirstContext _db;
    public DbFirstUserRepository(DbFirstContext db) => _db = db;

    public User? FindByUsername(string username)
    {
        var u = _db.Users.FirstOrDefault(x => x.Username == username);
        return u == null ? null : new User { Id = u.Id, Username = u.Username, PasswordHash = u.PasswordHash, Role = u.Role };
    }

    public void Add(User user)
    {
        _db.Users.Add(new DbFirstUser { Username = user.Username, PasswordHash = user.PasswordHash, Role = user.Role });
        _db.SaveChanges();
    }
}
