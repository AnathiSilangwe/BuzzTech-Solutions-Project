namespace BusinessBuzz.Core.Entities;

public class ApprovalReview
{
    public int ApprovalReviewId { get; set; }
    public int ApplicationId { get; set; }
    public int ApproverId { get; set; }
    public string Decision { get; set; } = string.Empty;
    public string ReasonText { get; set; } = string.Empty;

    // Navigation properties
    public Application Application { get; set; } = null!;
    public SystemUser Approver { get; set; } = null!;
    public void RecordDecision()
    {
        // Approval decision logic will be implemented later.
    }
}