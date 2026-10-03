using System.Reflection.Metadata;

namespace BusinessBuzz.Core.Entities;

public class BusinessProfile
{
    public int BusinessProfileId { get; set; }
    public int SystemUserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public int BusinessTypeId { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string BusinessLocation { get; set; } = string.Empty;
    public bool EmploysPeople { get; set; }
    public bool SeeksFunding {get; set; }
    public bool IsConfirmed { get; set; }

    //Navigation property
    public SystemUser SystemUser { get; set; } = null!;
    public BusinessType BusinessType { get; set; } = null!;
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<ComplianceRecord> ComplianceRecords { get; set; } = new List<ComplianceRecord>();
    public ICollection<ReadinessTask> ReadinessTasks { get; set; } = new List<ReadinessTask>();
    public ICollection<FundingAssessment> FundingAssessments { get; set; } = new List<FundingAssessment>();
    public ICollection<BuzzGuideMessage> BuzzGuideMessages { get; set; } = new List<BuzzGuideMessage>();

    public void CreateProfile()
    {
        // Logic to be implemented later
    }
    public void UpdateProfile()
    {
        // Logic to be implemented later
    }
}