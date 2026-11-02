public interface IUserStore
{
    User? FindByUsername(string username);
    void Add(User user);
}
