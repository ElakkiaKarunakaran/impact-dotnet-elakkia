using System;

class Program
{
	static void Main()
	{
		// Create service
		NotificationService service = new NotificationService();

		// Subscribe two listeners
		service.OnNotificationSent += (s, e) =>
			Console.WriteLine($"  [LOG] Sent to: {e.Recipient}");

		service.OnNotificationSent += (s, e) =>
			Console.WriteLine($"  [AUDIT] Message: {e.Message}");

		// Send using three different senders
		Console.WriteLine("--- Email ---");
		service.Send(Senders.SendEmail, "elakkia@email.com", "Welcome!");

		Console.WriteLine("\n--- SMS ---");
		service.Send(Senders.SendSMS, "9876543210", "OTP: 1234");

		Console.WriteLine("\n--- Push ---");
		service.Send(Senders.SendPush, "device-xyz", "New message!");
	}
}