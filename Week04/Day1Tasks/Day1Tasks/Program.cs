using Microsoft.Extensions.DependencyInjection;
// Manual DI 
var studentRepo = new InMemoryRepository<Student>(s => s.Id, (s, id) => s.Id = id);

// Seed some data
studentRepo.Seed(new List<Student>
{
	new Student { Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com" },
	new Student { Name="X",   Age=22, RollNumber="R002", Email="x@email.com"   },
	new Student { Name="Ani",   Age=23, RollNumber="R003", Email="ani@email.com"   }
});


var studentService = new StudentService(studentRepo);

// Step 3 — create view and controller, pass service and view in
var studentView = new StudentView();
var controller = new StudentController(studentService, studentView);

// Step 4 — run
controller.Run();

// To swap repo — just change ONE line above:
// var studentRepo = new SqlRepository<Student>(...);
// Nothing else changes


// Step 1 — create service collection (the registry)
var services = new ServiceCollection();

// Step 2 — register dependencies
// Singleton — one Logger for whole app
services.AddSingleton<IRepository<Student>>(sp =>
{
	var repo = new InMemoryRepository<Student>(
		s => s.Id,
		(s, id) => s.Id = id
	);
	repo.Seed(new List<Student>
	{
		new Student { Name="Elakkia", Age=21, RollNumber="R001", Email="elakkia@email.com" },
		new Student { Name="Ramya",   Age=22, RollNumber="R002", Email="ramya@email.com"   },
		new Student { Name="Arjun",   Age=23, RollNumber="R003", Email="arjun@email.com"   }
	});
	return repo;
});

// Transient — new service instance each time
services.AddTransient<IStudentService, StudentService>();

// Transient — new view each time
services.AddTransient<StudentView>();

// Transient — new controller each time
services.AddTransient<StudentController>();

// Step 3 — build the container
var provider = services.BuildServiceProvider();

// Step 4 — resolve controller (container creates everything automatically)
var controller = provider.GetRequiredService<StudentController>();

// Step 5 — run
controller.Run();

/*
WHY these lifetimes?
Repository = Singleton because we want ONE in-memory list for whole app
             If Transient, each resolution gets a NEW empty list — data lost!
Service    = Transient — stateless, safe to create fresh each time
View       = Transient — no state, safe to create fresh
Controller = Transient — no state, safe to create fresh
*/
