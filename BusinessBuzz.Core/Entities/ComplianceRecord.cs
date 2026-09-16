namespace BusinessBuzz.Core.Entities;

public class ComplianceRecord
{
  public int ComplianceRecordId { get; set; }
  public int BusinessProfileId { get; set; }
  public string ComplianceType { get; set; } = string.Empty;
  public DateTime? ExpiryDate { get; set; }

  //Navigation property 
  public BusinessProfile BusinessProfile { get; set; } = null!;
  public bool CheckExpiry()
    {
        if (!ExpiryDate.HasValue)
        {
            return false;
        }
        return ExpiryDate.Value < DateTime.UtcNow;
    }

}