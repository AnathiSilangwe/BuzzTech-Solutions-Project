namespace BusinessBuzz.Core.Entities;

public class Notification
{
    public int NotificationId { get; set; }
    public int SystemUserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    //Navigation property
    public SystemUser SystemUser { get; set; } = null!;
    public void SendNotification()
    {
        // Notification creation logic will be implemented later.
    }
    public void MarkAsRead()
    {
        IsRead = true;
    }
}