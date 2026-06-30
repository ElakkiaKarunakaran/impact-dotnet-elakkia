using order;

var X = "Hello";
//X = 90;
dynamic y = "Hello";
y = 90.00;
dynamic Add (dynamic a , dynamic b)
{
	return a+b ;
}
//tuple return
(int Min, int Max) GetMinMax(int[] numbers)
{
	int min = numbers[0];
	int max = numbers[0];
	foreach (int n in numbers)
	{
		if (n < min) min = n;
		if (n > max) max = n;
	}
	return (min, max);
}

var result = GetMinMax(new int[] { 5, 2, 9, 1, 7 });
Console.WriteLine($"Min: {result.Min}, Max: {result.Max}");

(string Name, int Age, string Department) GetEmployee()
{
	return ("Elakkia", 24, "Engineering");
}

// Deconstruction — unpack directly into 3 separate variables
var (name, age, department) = GetEmployee();
Console.WriteLine($"{name}, {age}, {department}");


Console.WriteLine(X);
Console.WriteLine(y);
Console.WriteLine(Add (2 , "Hello"));


string getscores(int score) => score switch
{
	>= 90 => "A",
	>= 80 =>"B",
	>= 70 => "C",
	>= 60 => "D",
	_ => "F"
};


double GetDiscount(Order order) => order switch
{
	{ Status: "VIP", Amount: > 1000 } => 0.20,
	{ Status: "VIP" } => 0.10,
	{ Status: "Regular", Amount: > 500 } => 0.05,
	_ => 0.0
};
var o1 = new Order { Status = "VIP", Amount = 1500 };
Console.WriteLine(GetDiscount(o1));  // 0.2
Console.WriteLine(getscores(90));

var s1 = new Student("Elakkia", 24);
Console.WriteLine($"{s1.Name}, {s1.Age}, enrolled {s1.EnrolledOn}");

var s2 = new Student("NewKid");  // uses chained constructor, defaults to MinAge
Console.WriteLine($"{s2.Name}, {s2.Age}");

try
{
	var bad = new Student("Invalid", 150);  // should throw
}
catch (ArgumentException ex)
{
	Console.WriteLine($"Validation caught: {ex.Message}");
}

// Const vs readonly - what you CANNOT do:
// Student.MinAge = 10;        // ERROR - const can never be changed, anywhere
// s1.EnrolledOn = DateTime.Now; // ERROR - readonly can only be set inside the constructor

