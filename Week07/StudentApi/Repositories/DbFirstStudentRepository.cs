using Microsoft.EntityFrameworkCore;
using StudentApi.Data.DbFirst;

// Task 7.10 — DB First repository: fulfils IRepository<Student>
// over the scaffolded DbFirstContext. Maps DbFirstStudent ↔ Student
// so the API surface is identical to the other two data layers.
public class DbFirstStudentRepository : IRepository<Student>
{
    private readonly DbFirstContext _db;

    public DbFirstStudentRepository(DbFirstContext db) => _db = db;

    public IEnumerable<Student> GetAll() =>
        _db.Students.AsNoTracking().Select(ToStudent).ToList();

    public Student? GetById(int id)
    {
        var s = _db.Students.AsNoTracking().FirstOrDefault(x => x.Id == id);
        return s == null ? null : ToStudent(s);
    }

    public void Add(Student student)
    {
        var entity = ToDbFirst(student);
        _db.Students.Add(entity);
        _db.SaveChanges();
        student.Id = entity.Id;
    }

    public void Update(Student student)
    {
        var entity = _db.Students.Find(student.Id);
        if (entity == null) return;
        entity.Name          = student.Name;
        entity.Age           = student.Age;
        entity.RollNumber    = student.RollNumber;
        entity.Email         = student.Email;
        entity.InternalNotes = student.InternalNotes;
        entity.EnrolledOn    = student.EnrolledOn;
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

    private static Student ToStudent(DbFirstStudent s) => new()
    {
        Id            = s.Id,
        Name          = s.Name,
        Age           = s.Age,
        RollNumber    = s.RollNumber,
        Email         = s.Email,
        InternalNotes = s.InternalNotes,
        EnrolledOn    = s.EnrolledOn
    };

    private static DbFirstStudent ToDbFirst(Student s) => new()
    {
        Id            = s.Id,
        Name          = s.Name,
        Age           = s.Age,
        RollNumber    = s.RollNumber,
        Email         = s.Email,
        InternalNotes = s.InternalNotes,
        EnrolledOn    = s.EnrolledOn
    };
}
