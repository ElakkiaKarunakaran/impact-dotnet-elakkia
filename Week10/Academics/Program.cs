using System.Text;
using Academics.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Task 10.3 — EF Code First DbContext (Scoped by default).
// "UseInMemoryDb" lets Academics.Tests swap in EF's InMemory provider without
// a live SQL Server — set via WebApplicationFactory test config, never in
// appsettings.json, so production always runs on real SQL Server.
if (builder.Configuration.GetValue<bool>("UseInMemoryDb"))
{
    builder.Services.AddDbContext<AcademicsContext>(o => o.UseInMemoryDatabase("AcademicsTestDb"));
}
else
{
    var connStr = builder.Configuration.GetConnectionString("AcademicsDb")!;
    builder.Services.AddDbContext<AcademicsContext>(o => o.UseSqlServer(connStr));
}

// Task 10.5 — Academics still validates the JWT locally.
// (Defense in depth: gateway validates too.)
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations on startup (dev-friendly; a real prod pipeline runs them separately).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademicsContext>();
    try { db.Database.Migrate(); } catch { /* DB not reachable yet; container will restart */ }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
