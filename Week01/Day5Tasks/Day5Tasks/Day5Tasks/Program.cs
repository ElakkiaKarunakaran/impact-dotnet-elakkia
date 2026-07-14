using System;
using System.Collections.Generic;
using System.Linq;

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

// Create 5 products
List<Product> products = new List<Product>
{
	new Product("Laptop", 75000, Category.Electronics),
	new Product("T-Shirt", 499, Category.Clothing),
	new Product("Phone", 25000, Category.Electronics),
	new Product("Rice Bag", 120, Category.Food),
	new Product("Jeans", 1299, Category.Clothing)
};

// Group by category and print
var grouped = products.GroupBy(p => p.Category);

foreach (var group in grouped)
{
	Console.WriteLine($"\nCategory: {group.Key}");
	Console.WriteLine("------------------");
	foreach (var product in group)
	{
		Console.WriteLine($"  {product.Name} - Rs.{product.Price}");
	}
}

TemperatureConverter converter = new TemperatureConverter();

// Convert from Celsius
converter.Convert(100, "C");

// Convert from Fahrenheit — pass true as second param
converter.Convert(212, true);

// Convert from Kelvin — pass 1 as second param
converter.Convert(373.15, 1);

// Create array of 5 contacts
ContactCard[] contacts = new ContactCard[5]
{
	new ContactCard("Elakkia", "9876543210", "elakkia@email.com"),
	new ContactCard("Sprity", "9123456789", "sprity@email.com"),
	new ContactCard("Ganesh", "9988776655", "Ganesh@email.com"),
	new ContactCard("A", "9001234567", "a@email.com"),
	new ContactCard("b", "9876001234", "B@email.com")
};

// Search function
void SearchContact(string query)
{
	Console.WriteLine($"\nSearching for: \"{query}\"");
	bool found = false;

	foreach (var contact in contacts)
	{
		if (string.Equals(contact.Name, query, StringComparison.OrdinalIgnoreCase))
		{
			Console.WriteLine("Contact found:");
			contact.Print();
			found = true;
			break;
		}
	}

	if (!found)
		Console.WriteLine("No contact found with that name.");
}

// Test case-insensitive search
SearchContact("elakkia");      // stored as "Elakkia" — should still find it
SearchContact("ganesh");        // stored as "" — should still find it
SearchContact("A");      // stored as "A" — should still find it
SearchContact("John");         // doesn't exist — should say not found