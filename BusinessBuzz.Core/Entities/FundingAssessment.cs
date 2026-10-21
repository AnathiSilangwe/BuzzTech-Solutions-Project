namespace BusinessBuzz.Core.Entities;

public class FundingAssessment
{
    public int FundingAssessmentId { get; set; }

    public int BusinessProfileId { get; set; }

    // A Banker is assigned when the assessment is recorded.
    // Pending enquiries can exist without an assigned Banker.
    public int? BankUserId { get; set; }

    // Workflow status: Pending or Completed.
    public string Status { get; set; } = "Pending";

    // Assessment outcome: Approved, Declined or More information required.
    public string SimulatedOutcome { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? AssessedAt { get; set; }

    // Navigation properties
    public BusinessProfile BusinessProfile { get; set; } = null!;

    public SystemUser? BankUser { get; set; }

    public void RunAssessment()
    {
        // The Banker records a simulated assessment through the portal.
        // This does not represent a real bank's lending decision.
    }
}
