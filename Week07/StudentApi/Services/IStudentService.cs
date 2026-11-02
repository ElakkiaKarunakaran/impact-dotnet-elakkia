public interface IStudentService
{
	(bool Success, string Message) AddStudent(Student student);
	IEnumerable<Student> GetAll();
	Student GetById(int id);
	(bool Success, string Message) UpdateStudent(Student student);
	(bool Success, string Message) DeleteStudent(int id);
	IEnumerable<string> GetTransactionLog();
	IEnumerable<Student> Search(string name);

}