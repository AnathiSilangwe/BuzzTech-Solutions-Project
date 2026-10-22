namespace BusinessBuzz.Core.Entities;

public class UserRole
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    //Navigation property
    public ICollection<SystemUser> SystemUsers { get; set; } = new List<SystemUser>();
    public ICollection<string> GetPermission()
    {
        return RoleName switch
        {
            "Entrepreneur" => new List<string>
            {
                "ViewDashboard",
                "ManageBusinessProfile",
                "ViewJourney",
                "ManageDocuments",
                "ViewCompliance",
                "ViewBusinessReadiness",
                "MakeFundingEnquiry"
            },
            "Approver" => new List<string>
            {
                "ReviewApplications",
                "ReviewDocuments",
                "ApproveApplication",
                "RejectApplication",
                "RequestAdditionalInformation",
                "UpdateWorkflowStatus"
            },
            // "SARS Approver" => new List<string>
            // {
            //     "Review SARS Applications",
            //     "ReviewDocuments",
            //     "ApproveApplication",
            //     "RejectApplication",
            //     "RequestAdditionalInformation",
            //     "UpdateWorkflowStatus"
            // },
            "Bank User" => new List<string>
            {
                "ViewBusinesses",
                "ReviewFundingEnquiries",
                "ReviewDocuments",
                "PerformFundingAssessment",
                "RecordAssessmentOutcome"
            },

            _ => new List<string>()
        };
        
    }

}