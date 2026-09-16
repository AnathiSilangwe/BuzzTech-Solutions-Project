namespace BusinessBuzz.Core.Entities;

public class Application
{
    public int ApplicationId { get; set; }
    public int BusinessProfileId { get; set; }
    public int JourneyStageId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmissionDate {get; set;}

    //Navigation Property
    public BusinessProfile BusinessProfile {get; set;} = null!;
    public JourneyStage JourneyStage { get; set; } = null!;
    public ICollection<ApprovalReview> ApprovalReviews { get; set; } = new List<ApprovalReview>();
    public ICollection<InformationRequest> InformationRequests { get; set; } = new List<InformationRequest>();
    public void SubmitApplication()
    {
        Status = "Submitted";
        SubmissionDate = DateTime.UtcNow;
    }
    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
    }
}