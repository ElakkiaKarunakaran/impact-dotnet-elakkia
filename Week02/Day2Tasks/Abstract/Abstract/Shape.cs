
public abstract class Shape
{

	public string color { get; set; }

	//abstract class
	public abstract double CalculateArea();

	//concrete class
	public void DisplayArea()
	{
		Console.WriteLine($"Area = {CalculateArea()}");
	}

}

public class Circle : Shape
{
	public double Radius { get; set; }

	public Circle(double raidus) => Radius = raidus;

	public override double CalculateArea() 
	{ 
		dynamic area = Math.PI * Radius * Radius;
		Console.WriteLine($"Area ={area}");
		return area;
	}
	

}

public class Rectangle : Shape
{

	public double Length { get; set; }

	public double Breadth { get; set; }

	public Rectangle(double length, double breadth)
	{
		Length = length;
		Breadth = breadth;
	}

	public override double CalculateArea() => Length * Breadth;

}