Circle c = new Circle(10);
c.CalculateArea();

//Shape s = new Shape(); // ERROR: Cannot create an instance of the abstract class 'Shape'


Triangle t = new Triangle(10,9,8);
t.CalculateArea();

IShape shapeRef = t;
Console.WriteLine(shapeRef.CalculateArea());

IDrawable drawRef = t;
drawRef.Draw();


// Calculator overloading
var calc = new Calculator();
Console.WriteLine(calc.Add(2, 3));           // int version → 5
Console.WriteLine(calc.Add(2.5, 3.5));       // double version → 6
Console.WriteLine(calc.Add(1, 2, 3));        // 3-arg version → 6
Console.WriteLine(calc.Add(1, 2, 3, 4, 5)); // params version → 15

// Runtime polymorphism — List<Shape>
// Uses Shape from Task 2.4
List<Shape> shapes = new List<Shape>
{
	new Circle(5),
	new Rectangle(4, 6)
};

foreach (var shape in shapes)
{
	// runtime picks Circle's or Rectangle's CalculateArea() automatically
	Console.WriteLine($"{shape.GetType().Name}: Area = {shape.CalculateArea():F2}");
}

// Method hiding — new vs override
Logger loggerRef = new FileLogger();
loggerRef.Log("test");
// Prints: [Logger] test
// Because variable is Logger type, and FileLogger used "new" not "override"
// So Logger's version runs — method hiding in action

FileLogger fileRef = new FileLogger();
fileRef.Log("test");
// Prints: [FileLogger] Writing to file: test
// Because variable is FileLogger type, so FileLogger's hidden version runs

/*
SUMMARY:
override → actual object type decides (runtime polymorphism)
new      → variable declaration type decides (compile-time hiding)
*/


var m1 = new Money(500, "INR");
var m2 = new Money(300, "INR");
var m3 = new Money(100, "USD");

// Addition — same currency, works fine
var total = m1 + m2;
Console.WriteLine(total);       // INR 800

// Comparison
Console.WriteLine(m1 > m2);    // True
Console.WriteLine(m1 < m2);    // False
Console.WriteLine(m1 == m2);   // False
Console.WriteLine(m1 != m2);   // True

// Mismatched currency — throws
try
{
	var bad = m1 + m3;
}
catch (InvalidOperationException ex)
{
	Console.WriteLine($"Error: {ex.Message}");
	// Error: Cannot add INR and USD — currency mismatch.
}