using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentApi.Data.DbFirst;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Task 8.9 — CORS: allow the React dev server origin
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "DELETE")));

// Task 8.7 — JWT Bearer authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

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
        builder.Services.AddScoped<IUserStore, EfUserRepository>();
        break;

    case "DbFirst":
        builder.Services.AddDbContext<DbFirstContext>(o =>
            o.UseSqlServer(connStr));
        builder.Services.AddScoped<IRepository<Student>, DbFirstStudentRepository>();
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

// inject Jwt config so AuthController can generate tokens
builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");        // Task 8.9 — before auth
app.UseAuthentication();        // Task 8.7
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
