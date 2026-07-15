public class NotificationEventArgs : EventArgs
{
	public string Recipient { get; set; }

	public string Message { get; set; }


	public NotificationEventArgs(string recipient, string message)
	{
		Recipient = recipient;
		Message = message;


	}

	}