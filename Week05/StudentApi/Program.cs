using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository — Singleton so data persists
builder.Services.AddSingleton<IRepository<Student>>(sp =>
{
	var repo = new InMemoryRepository<Student>(
		s => s.Id, (s, id) => s.Id = id);
	repo.Seed(new List<Student>
	{
		new Student { Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com" },
		new Student { Name="Ramya",   Age=22, RollNumber="R002", Email="ramya@email.com"   },
		new Student { Name="Arjun",   Age=23, RollNumber="R003", Email="arjun@email.com"   }
	});
	return repo;
});

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