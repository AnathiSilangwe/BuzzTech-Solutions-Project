

namespace BusinessBuzz.Core.Entities;

public class SystemUser
{
    public int SystemUserId { get; set; }
    public string Name {get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleId { get; set; }
    
    //Navigation property
    public UserRole Role { get; set; } = null!;
    public ICollection<BusinessProfile> BusinessProfiles { get; set; } = new List<BusinessProfile>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<FundingAssessment> FundingAssessments { get; set; } = new List<FundingAssessment>();
    public ICollection<ApprovalReview> ApprovalReviews { get; set; } = new List<ApprovalReview>();
   
}