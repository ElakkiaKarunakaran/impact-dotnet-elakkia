public class EvenNumbers
{
	// IEnumerable<int> means "this method produces a sequence of ints"
	// yield return means "hand over this one item, then PAUSE until next is needed"
	public static IEnumerable<int> GetEvenNumbers(int max)
	{
		Console.WriteLine("-- Generator started --");

		for (int i = 2; i <= max; i += 2)
		{
			Console.WriteLine($"  Yielding {i}");
			yield return i; // pauses here, resumes when foreach asks for next item
		}

		Console.WriteLine("-- Generator finished --");
	}
}