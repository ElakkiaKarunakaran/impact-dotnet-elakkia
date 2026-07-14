public class AccessMatrix
{
	public string PublicField = "Anyone can see me";
	private string privateField = "Only I can see me";
	protected string protectedField = "Me and my children";
	internal string internalField = "Anywhere in this project";

	public void ShowAll()
	{
		// inside the class, all fields are accessible
		Console.WriteLine(PublicField);
		Console.WriteLine(privateField);
		Console.WriteLine(protectedField);
		Console.WriteLine(internalField);
	}
}

