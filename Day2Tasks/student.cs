namespace SchoolManagement
{
	public class Student
	{
		#region Fields
		private int age;
		#endregion

		#region Properties
		public string Name { get; set; }
		#endregion

		#region Constructors
		public Student() { }
		#endregion

		#region Methods
		public void SayHello()
		{
			Console.WriteLine($"Hello, I'm {Name}");
		}
		#endregion
	}
}