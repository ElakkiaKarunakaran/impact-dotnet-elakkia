using Microsoft.EntityFrameworkCore;
using StudentApi.Data.DbFirst;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Task 7.1 — connection string from IConfiguration (never hard-coded)
var connStr   = builder.Configuration.GetConnectionString("StudentDb")!;
var dataLayer = builder.Configuration["DataLayer"] ?? "InMemory";

// Task 7.11 — swappable data layer: change DataLayer in appsettings.json only
switch (dataLayer)
{
    case "AdoNet":
        builder.Services.AddScoped<IRepository<Student>>(
            _ => new AdoNetStudentRepository(connStr));
        builder.Services.AddScoped<IUserStore>(
            _ => new AdoNetUserRepository(connStr));
        break;

    case "EfCodeFirst":
        builder.Services.AddDbContext<AppDbContext>(o =>
            o.UseSqlServer(connStr));
        builder.Services.AddScoped<IRepository<Student>, EfStudentRepository>();
        // EF Code First user store (reuses same context)
        builder.Services.AddScoped<IUserStore, EfUserRepository>();
        break;

    case "DbFirst":
        builder.Services.AddDbContext<DbFirstContext>(o =>
            o.UseSqlServer(connStr));
        builder.Services.AddScoped<IRepository<Student>, DbFirstStudentRepository>();
        // DB-First user store
        builder.Services.AddScoped<IUserStore, DbFirstUserRepository>();
        break;

    default: // "InMemory"
        builder.Services.AddSingleton<IRepository<Student>>(sp =>
        {
            var repo = new InMemoryRepository<Student>(s => s.Id, (s, id) => s.Id = id);
            repo.Seed(new List<Student>
            {
                new() { Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com" },
                new() { Name="Ramya",   Age=22, RollNumber="R002", Email="ramya@email.com"   },
                new() { Name="Arjun",   Age=23, RollNumber="R003", Email="arjun@email.com"   }
            });
            return repo;
        });
        builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
        break;
}

// Service — Scoped (one per HTTP request)
builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Required for test project's WebApplicationFactory
public partial class Program { }
