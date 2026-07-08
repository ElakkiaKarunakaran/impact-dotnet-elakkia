
/*
ABSTRACT CLASS vs INTERFACE

Abstract class:
- Can have both abstract (no body) AND concrete (with body) methods
- A class can only inherit ONE abstract class
- Can have fields, constructors, access modifiers
- Use when classes share common code AND a common identity (Shape → Circle, Rectangle)

Interface:
- Only method signatures — no body (before C# 8), no fields, no constructors
- A class can implement MULTIPLE interfaces
- All members are public by default
- Use when unrelated classes need to guarantee the same behaviour
  e.g. a Circle and a PDFDocument can both be IDrawable even though they're unrelated
*/


public interface IShape { 
 double CalculateArea();
double CalculatePerimeter();
}

public interface IDrawable {

	void Draw();
}

public class Triangle : IDrawable , IShape 
{
	public double A { get; set; }
	public double B { get; set; }
	public double C { get; set; }

	public Triangle(double a, double b, double c)
	{
		A = a; B = b; C = c;
	}

	public double CalculateArea()
	{
		double s = (A + B + C) / 2;
		Console.WriteLine($"{s}");
		return Math.Sqrt(s * (s - A) * (s - B) * (s - C));


	}

	public double CalculatePerimeter() => A + B + C;


	public void Draw()
	{
		Console.WriteLine("Drawing a triangle...");
	}

}