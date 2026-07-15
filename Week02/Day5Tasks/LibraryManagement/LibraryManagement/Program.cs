using System.Text.RegularExpressions;

List<Book> books = new List<Book>
{
			new Book("The Alchemist",            "Paulo Coelho",   "Fiction",    1988, true),
			new Book("Atomic Habits",            "James Clear",    "Self-Help",  2018, true),
			new Book("Clean Code",               "Robert Martin",  "Technology", 2008, false),
			new Book("Ikigai",                   "Hector Garcia",  "Self-Help",  2016, true),
			new Book("The Pragmatic Programmer", "Robert Martin",  "Technology", 1999, true),
			new Book("Harry Potter 1",           "J.K. Rowling",   "Fiction",    1997, false),
			new Book("Harry Potter 2",           "J.K. Rowling",   "Fiction",    1998, true),
			new Book("Deep Work",                "Cal Newport",    "Self-Help",  2016, true),
			new Book("Design Patterns",          "Gang of Four",   "Technology", 1994, false),
			new Book("The Great Gatsby",         "F. Scott",       "Fiction",    1925, true),
			new Book("Rich Dad Poor Dad",        "R. Kiyosaki",    "Finance",    1997, true),
			new Book("Think and Grow Rich",      "Napoleon Hill",  "Finance",    1937, true),
			new Book("Zero to One",              "Peter Thiel",    "Finance",    2014, true),
			new Book("The Lean Startup",         "Eric Ries",      "Finance",    2011, false),
			new Book("Refactoring",              "Robert Martin",  "Technology", 2018, true)

};


Console.WriteLine("Available books ");

var query_1 = books
	 .Where(b=>b.IsAvailable == true && b.Author == "Robert Martin");

foreach (var b in query_1)
{
	Console.WriteLine($"{b.Title} , {b.Author}");

}

// ── Query 2 ──────────────────────────────
Console.WriteLine("\n=== Books by Genre ===");

var query2 = books
	.GroupBy(b => b.Genre)
	.Select(g => new
	{
		Genre = g.Key,      // the genre name
		Count = g.Count()   // how many books in this genre
	});

foreach (var g in query2)
	Console.WriteLine($"  {g.Genre}: {g.Count} books");


var query3 = books.OrderBy(s => s.Year)
	.First();



Console.WriteLine($"  {query3.Title} by {query3.Author} ({query3.Year})");

// ── Query 4 ──────────────────────────────
Console.WriteLine("\n=== Books After 2010 (A-Z) ===");

var query4 = books
	.Where(b => b.Year > 2010)      // only after 2010
	.OrderBy(b => b.Title);          // sort A-Z by title

foreach (var b in query4)
	Console.WriteLine($"  {b.Title} ({b.Year})");



