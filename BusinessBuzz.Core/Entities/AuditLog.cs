namespace BusinessBuzz.Core.Entities;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public int SystemUserId { get; set; }
    public string ActionPerformed { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    //Navigation property
    public SystemUser SystemUser { get; set; } = null!;
    public void RecordLog()
    {
        Timestamp = DateTime.UtcNow;
    }
}