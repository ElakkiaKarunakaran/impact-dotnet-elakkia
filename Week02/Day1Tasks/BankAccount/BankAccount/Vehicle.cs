public class Vehicle
{
	public string Make { get; set; }
	public string Model { get; set; }
	public int Year { get; set; }

	public Vehicle(string make, string model, int year)
	{
		Make = make;
		Model = model;
		Year = year;
	}

	public virtual void DisplayInfo()
	{
		Console.WriteLine($"Vehicle: {Year} {Make} {Model}");
	}
}

public class Car : Vehicle
{
	public int NumberOfDoors { get; set; }

	public Car(string make, string model, int year, int doors)
		: base(make, model, year)  // calls Vehicle's constructor first
	{
		NumberOfDoors = doors;
	}

	public override void DisplayInfo()
	{
		base.DisplayInfo(); // print the vehicle info first
		Console.WriteLine($"  Doors: {NumberOfDoors}");
	}
}

public class Bike : Vehicle
{
	public bool HasSidecar { get; set; }

	public Bike(string make, string model, int year, bool hasSidecar)
		: base(make, model, year)
	{
		HasSidecar = hasSidecar;
	}

	public override void DisplayInfo()
	{
		base.DisplayInfo();
		Console.WriteLine($"  Sidecar: {(HasSidecar ? "Yes" : "No")}");
	}
}

public class ElectricCar : Car
{
	public int BatteryCapacity { get; set; }

	public ElectricCar(string make, string model, int year, int doors, int battery)
		: base(make, model, year, doors)  // calls Car's constructor, which calls Vehicle's
	{
		BatteryCapacity = battery;
	}

	public override void DisplayInfo()
	{
		base.DisplayInfo(); // calls Car's DisplayInfo
		Console.WriteLine($"  Battery: {BatteryCapacity} kWh");
	}
}