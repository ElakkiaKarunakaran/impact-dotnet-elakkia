public class NotificationService
{ 
public event EventHandler <NotificationEventArgs> OnNotificationSent;


	public void Send(NotificationSender sender, string recipient, string message)
	{
		sender(recipient, message);
		OnNotificationSent?.Invoke(this,
	 new NotificationEventArgs(recipient, message));

	}
}