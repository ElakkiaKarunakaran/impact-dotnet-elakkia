public static class Helpers
{
    public static void ParseAndDivide(string input, int divisor)
    {
        try
        {
            int number = int.Parse(input);  // can throw FormatException
            int result = checked(number / divisor); // can throw OverflowException or DivideByZero
            Console.WriteLine($"Result: {result}");
        }
        catch (FormatException ex)
        {
            // MUST come before Exception
            Console.WriteLine($"Format error: {ex.Message}");
        }
        catch (OverflowException ex)
        {
            // MUST come before Exception
            Console.WriteLine($"Overflow error: {ex.Message}");
        }
        catch (DivideByZeroException ex)
        {
            // MUST come before Exception
            Console.WriteLine($"Divide by zero: {ex.Message}");
        }
        catch (Exception ex)
        {
            // MUST come LAST — catches everything else
            Console.WriteLine($"General error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("ParseAndDivide attempt completed");
        }
    }
}

// PROOF that Exception first causes compile error:
// Try swapping Exception to the top — VS will show:
// "A previous catch clause already catches all exceptions"