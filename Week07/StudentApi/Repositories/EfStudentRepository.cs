using Microsoft.EntityFrameworkCore;

// Task 7.7 — EF Core Code First implementation of IRepository<Student>
// Swap this in behind the API: DataLayer = "EfCodeFirst" in appsettings.json
public class EfStudentRepository : IRepository<Student>
{
    private readonly AppDbContext _db;

    public EfStudentRepository(AppDbContext db) => _db = db;

    public IEnumerable<Student> GetAll() => _db.Students.AsNoTracking().ToList();

    public Student? GetById(int id) => _db.Students.AsNoTracking().FirstOrDefault(s => s.Id == id);

    public void Add(Student student)
    {
        _db.Students.Add(student);
        _db.SaveChanges();
    }

    public void Update(Student student)
    {
        _db.Students.Update(student);
        _db.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _db.Students.Find(id);
        if (entity != null)
        {
            _db.Students.Remove(entity);
            _db.SaveChanges();
        }
    }
}
