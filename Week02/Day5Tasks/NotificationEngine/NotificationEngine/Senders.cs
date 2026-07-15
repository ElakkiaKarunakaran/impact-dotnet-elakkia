public class Senders
{
	public static void SendEmail(string recipient, string message)
	{

		Console.WriteLine($"Email to :{recipient} , { message}");
	}


	public static void SendSMS ( string recipient, string message ) {
		Console.WriteLine($"SMS to , {recipient} ,{message}");
	}

	public static void SendPush(string recipient, string message)
	{
		Console.WriteLine($"[PUSH] To: {recipient} | {message}");
	}
}