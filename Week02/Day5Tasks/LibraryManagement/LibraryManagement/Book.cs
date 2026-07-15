public class Book
{

	public string Title { get; set; }
	public string Author { get; set; }
	public string Genre { get; set; }
	public int Year { get; set; }
	public bool IsAvailable { get; set; }
	public Book(string title , string author , string genre , int year , bool isavailable)
	{
		Title = title;
		Author = author;
		Genre = genre;
		Year = year;
		IsAvailable = isavailable;
	}
}