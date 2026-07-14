public partial class Employee
{
	public string Name { get; set; }
	public int Id { get; set; }

	// partial method - just the signature here, body goes in the other file
	partial void OnNameChanged(string newName);

	public void ChangeName(string newName)
	{
		Name = newName;
		OnNameChanged(newName); // calls the partial method
	}
}