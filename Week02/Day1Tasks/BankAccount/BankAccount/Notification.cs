public class Notification
{
	public virtual void Send()
	{
		Console.WriteLine("Sending generic notification...");
	}
}

public class EmailNotification : Notification
{
	public sealed override void Send()  // sealed = no further overriding allowed
	{
		Console.WriteLine("Sending EMAIL notification...");
	}
}

public class SmsNotification : Notification
{
	public override void Send()
	{
		Console.WriteLine("Sending SMS notification...");
	}
}

public class PushNotification : Notification
{
	public sealed override void Send()
	{
		Console.WriteLine("Sending PUSH notification...");
	}
}

//public class FancyEmail : PushNotification
//{
//	public override void Send()  // ERROR: Send is sealed in EmailNotification
//	{
//		Console.WriteLine("Fancy email");
//	}
//}