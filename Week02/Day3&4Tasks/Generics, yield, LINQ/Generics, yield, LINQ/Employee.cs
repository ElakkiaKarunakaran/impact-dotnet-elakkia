public class Employee
{
	public string Name { get; set; }
	public string Department { get; set; }
	public double Salary { get; set; }
	public DateTime JoiningDate { get; set; }

	public Employee(string name, string dept, double salary, DateTime joiningDate)
	{
		Name = name;
		Department = dept;
		Salary = salary;
		JoiningDate = joiningDate;
	}
}