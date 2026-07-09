Console.WriteLine("Before foreach");

foreach (int n in EvenNumbers.GetEvenNumbers(10))
{
	Console.WriteLine($"Received: {n}");
}

Console.WriteLine("After foreach");

var collection = new BookCollection();
collection.AddBook(new Book("Zebra Tales", "Author C"));
collection.AddBook(new Book("Apple Stories", "Author A"));
collection.AddBook(new Book("Mango Days", "Author B"));

Console.WriteLine("\n-- Books in alphabetical order --");
foreach (Book b in collection) // uses GetEnumerator() automatically
{
	Console.WriteLine($"{b.Title} by {b.Author}");
}