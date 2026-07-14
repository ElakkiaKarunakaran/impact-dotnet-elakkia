var emp = new Employee { Id = 1, Name = "Elakkia", Department = "Engineering" };
emp.PrintDetails();
emp.ChangeName("Elakkia K");
emp.PrintDetails();
// Even though Employee is split across 2 files, it works as one class


var matrix = new AccessMatrix();
Console.WriteLine(matrix.PublicField);    // works fine
Console.WriteLine(matrix.internalField);  // works — same project
										  //Console.WriteLine(matrix.privateField); // ERROR — private, can't access from outside
matrix.ShowAll(); // the class itself can access all its own fields


var addr1 = new Address("12 MG Road", "Chennai", "600001");
var addr2 = new Address("12 MG Road", "Chennai", "600001");
var addr3 = new Address("5 Anna Salai", "Chennai", "600002");

// Value equality — records compare property by property, not by reference
Console.WriteLine(addr1 == addr2);  // True — same values
Console.WriteLine(addr1 == addr3);  // False — different Street and Pincode

// "with" — creates a copy with only Pincode changed, addr1 stays untouched
var addr4 = addr1 with { Pincode = "600010" };
Console.WriteLine(addr1.Pincode);  // 600001 — original unchanged
Console.WriteLine(addr4.Pincode);  // 600010 — new copy with change


// For comparison: two regular class objects with same values are NOT equal
// because == checks if they're the same object in memory, not if values match

var playlist = new Playlist();
playlist.AddSong("Song A");
playlist.AddSong("Song B");
playlist.AddSong("Song C");

Console.WriteLine(playlist[0]);           // Song A — int indexer
Console.WriteLine(playlist["Song B"]);    // 1 — string indexer returns position

playlist[1] = "Song B Remix";            // update using int indexer
Console.WriteLine(playlist[1]);           // Song B Remix

try
{
	Console.WriteLine(playlist[99]);      // triggers bounds check
}
catch (IndexOutOfRangeException ex)
{
	Console.WriteLine($"Error: {ex.Message}");
}