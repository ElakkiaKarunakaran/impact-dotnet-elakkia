var logger1 = Logger.Instance;
var logger2 = Logger.Instance;

logger1.Log("Hello from logger 1");
logger2.Log("Hello from logger 2");

Console.WriteLine($"Logger 1: {logger1.GetHashCode()}");
Console.WriteLine($"Logger 2: {logger2.GetHashCode()}");

SingletonDemo.Run();