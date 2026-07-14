public class TemperatureConverter
{
	// Overload 1 — input is Celsius
	public void Convert(double celsius, string unit = "C")
	{
		double fahrenheit = (celsius * 9 / 5) + 32;
		double kelvin = celsius + 273.15;

		Console.WriteLine($"\nInput: {celsius}°C");
		Console.WriteLine($"  Fahrenheit : {fahrenheit:F2}°F");
		Console.WriteLine($"  Kelvin     : {kelvin:F2}K");
	}

	// Overload 2 — input is Fahrenheit
	public void Convert(double fahrenheit, bool isFahrenheit)
	{
		double celsius = (fahrenheit - 32) * 5 / 9;
		double kelvin = celsius + 273.15;

		Console.WriteLine($"\nInput: {fahrenheit}°F");
		Console.WriteLine($"  Celsius : {celsius:F2}°C");
		Console.WriteLine($"  Kelvin  : {kelvin:F2}K");
	}

	// Overload 3 — input is Kelvin
	public void Convert(double kelvin, int isKelvin)
	{
		double celsius = kelvin - 273.15;
		double fahrenheit = (celsius * 9 / 5) + 32;

		Console.WriteLine($"\nInput: {kelvin}K");
		Console.WriteLine($"  Celsius    : {celsius:F2}°C");
		Console.WriteLine($"  Fahrenheit : {fahrenheit:F2}°F");
	}
}