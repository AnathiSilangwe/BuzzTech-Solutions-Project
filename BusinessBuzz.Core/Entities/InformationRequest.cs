namespace BusinessBuzz.Core.Entities;

public class InformationRequest
{
    public int InformationRequestId { get; set; }
    public int ApplicationId { get; set; }
    public int ApproverId { get; set; }
    public string RequestMessage { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime RequestedDate { get; set; }
    public DateTime? ResponseDate { get; set; }

    //Navigation property
    public Application Application { get; set; } = null!;
    public SystemUser Approver { get; set; } = null!;
    public void RecordResponse(string response)
    {
        Response = response;
        Status = "Responded";
        ResponseDate = DateTime.UtcNow;
    }
}