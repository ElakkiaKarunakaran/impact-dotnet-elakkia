public class Calculator
{
    // Overload 1 — two ints
    public int Add(int a, int b) => a + b;

    // Overload 2 — two doubles
    public double Add(double a, double b) => a + b;

    // Overload 3 — three ints
    public int Add(int a, int b, int c) => a + b + c;

    // Overload 4 — params (any number of ints)
    public int Add(params int[] numbers)
    {
        int sum = 0;
        foreach (var n in numbers) sum += n;
        return sum;
    }
}