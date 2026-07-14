public struct ContactCard
{
	public string Name { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }

	public ContactCard(string name, string phone, string email)
	{
		Name = name;
		Phone = phone;
		Email = email;
	}

	public void Print()
	{
		Console.WriteLine($"  Name  : {Name}");
		Console.WriteLine($"  Phone : {Phone}");
		Console.WriteLine($"  Email : {Email}");
	}
}