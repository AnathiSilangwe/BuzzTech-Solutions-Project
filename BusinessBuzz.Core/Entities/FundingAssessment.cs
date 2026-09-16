namespace BusinessBuzz.Core.Entities;

public class FundingAssessment
{
    public int FundingAssessmentId { get; set; }
    public int BusinessProfileId { get; set; }
    public int BankUserId { get; set; }
    public string SimulatedOutcome { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;

    //Navigation Property
    public BusinessProfile BusinessProfile { get; set; } = null!;
    public SystemUser BankUser { get; set; } = null!;

    public void RunAssessment()
    {
        //Simulated envieonment implementations
    }
}