public class StudentController
{
	private readonly IStudentService service;
	private readonly StudentView view;

	public StudentController(IStudentService service, StudentView view)
	{
		this.service = service;
		this.view = view;
	}

	public void Run()
	{
		bool running = true;
		while (running)
		{
			view.ShowMenu();
			string choice = Console.ReadLine();

			switch (choice)
			{
				case "1": AddStudent(); break;
				case "2": ViewAll(); break;
				case "3": FindById(); break;
				case "4": UpdateStudent(); break;
				case "5": DeleteStudent(); break;
				case "0": running = false; break;
				default:
					view.ShowMessage("Invalid choice");
					break;
			}
		}
		view.ShowMessage("Goodbye!");
	}

	// Each method: get input from view → call service → show result via view
	// Controller NEVER formats output or applies business rules

	private void AddStudent()
	{
		var student = view.PromptForStudent();
		var (success, message) = service.AddStudent(student);
		view.ShowMessage(message);
	}

	private void ViewAll()
	{
		var students = service.GetAll();
		view.PrintStudents(students);
	}

	private void FindById()
	{
		int id = view.PromptForId();
		var student = service.GetById(id);
		view.PrintStudent(student);
	}

	private void UpdateStudent()
	{
		int id = view.PromptForId("Enter ID to update: ");
		var existing = service.GetById(id);
		if (existing == null)
		{
			view.ShowMessage($"Student with ID {id} not found");
			return;
		}
		var updated = view.PromptForStudent();
		updated.Id = id;
		var (success, message) = service.UpdateStudent(updated);
		view.ShowMessage(message);
	}

	private void DeleteStudent()
	{
		int id = view.PromptForId("Enter ID to delete: ");
		var (success, message) = service.DeleteStudent(id);
		view.ShowMessage(message);
	}

	
}