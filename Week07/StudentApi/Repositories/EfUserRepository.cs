// EF Code First implementation of IUserStore
public class EfUserRepository : IUserStore
{
    private readonly AppDbContext _db;
    public EfUserRepository(AppDbContext db) => _db = db;

    public User? FindByUsername(string username) =>
        _db.Users.FirstOrDefault(u => u.Username == username);

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }
}
