var account = new BankAccount(1000);
account.Deposit(500);
account.Withdraw(200);
account.Withdraw(5000); // should be rejected — overdraw
account.Deposit(-100);  // should be rejected — negative deposit
Console.WriteLine($"Final balance: Rs.{account.GetBalance()}");
account.PrintHistory();


var car = new Car("Toyota", "Camry", 2022, 4);
car.DisplayInfo();

Console.WriteLine();

var bike = new Bike("Harley", "Sportster", 2021, true);
bike.DisplayInfo();

Console.WriteLine();

var ev = new ElectricCar("Tesla", "Model 3", 2023, 4, 75);
ev.DisplayInfo();
// Constructor chain: ElectricCar → Car → Vehicle (top to bottom)


Notification n1 = new EmailNotification();
Notification n2 = new SmsNotification();
Notification n3 = new PushNotification();

n1.Send();
n2.Send();
n3.Send();

// To see the sealed error: uncomment the FancyEmail class above and try to build
// Error will say: "cannot override inherited member EmailNotification.Send()
// because it is sealed"