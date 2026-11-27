using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reporting.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Task 10.4 — EF DB First context; the DB schema is authored externally
// (sql/reporting-init.sql) and these classes merely map onto it.
// "UseInMemoryDb" lets Reporting.Tests swap in EF's InMemory provider for the
// Enrollments-table endpoints without a live SQL Server (see Program.cs comment
// in Academics for why this beats descriptor-removal in WithWebHostBuilder).
if (builder.Configuration.GetValue<bool>("UseInMemoryDb"))
{
    builder.Services.AddDbContext<ReportingContext>(o => o.UseInMemoryDatabase("ReportingTestDb"));
}
else
{
    var connStr = builder.Configuration.GetConnectionString("ReportingDb")!;
    builder.Services.AddDbContext<ReportingContext>(o => o.UseSqlServer(connStr));
}

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
