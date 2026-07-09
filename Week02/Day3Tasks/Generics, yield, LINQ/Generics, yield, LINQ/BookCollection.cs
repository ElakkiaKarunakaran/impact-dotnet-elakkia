public class Book
{
	public string Title { get; set; }
	public string Author { get; set; }

	public Book(string title, string author)
	{
		Title = title;
		Author = author;
	}
}

// IEnumerable<Book> means this class can be used directly in foreach
public class BookCollection : IEnumerable<Book>
{
	private List<Book> books = new List<Book>();

	public void AddBook(Book book) => books.Add(book);

	// This is the method IEnumerable requires you to implement
	// It yields books in alphabetical order by title
	public IEnumerator<Book> GetEnumerator()
	{
		// Sort alphabetically first
		var sorted = books.OrderBy(b => b.Title);

		foreach (var book in sorted)
		{
			yield return book; // lazy — one book at a time
		}
	}

	// Required by IEnumerable — just calls the typed version above
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> GetEnumerator();
}