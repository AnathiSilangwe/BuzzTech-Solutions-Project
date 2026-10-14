
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Approver;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<Application> SubmittedApplications { get; private set; }
        = new();

    public List <Application> ReviewedApplications {get; private set; } = new();
    [TempData]
    public string? SuccessMessage{ get; set; }

    [BindProperty]
    public int ApplicationId { get; set; }

    [BindProperty]
    public string Decision { get; set; } = string.Empty;

    [BindProperty]
    public string ReasonText { get; set; } = string.Empty;

    public string? Message { get; private set; }

    private bool IsApprover()
    {
        return HttpContext.Session.GetInt32("RoleId") == 2;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        if (!IsApprover())
            return Forbid();

        await LoadApplicationsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostRecordDecisionAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        if (!IsApprover())
            return Forbid();

        if (Decision != "Approved" &&
            Decision != "Rejected" &&
            Decision != "Action required")
        {
            return BadRequest("Invalid application decision.");
        }

        if ((Decision == "Rejected" || Decision == "Action required") &&
            string.IsNullOrWhiteSpace(ReasonText))
        {
            Message = "Please provide a reason for this decision.";
            await LoadApplicationsAsync();
            return Page();
        }

        var application = await _context.Applications
            .Include(a => a.BusinessProfile)
            .Include(a => a.JourneyStage)
            .FirstOrDefaultAsync(a => a.ApplicationId == ApplicationId);

        if (application == null)
            return NotFound();

        // Only decide on applications currently awaiting review.
        if (application.Status != "Submitted")
        {
            Message = "This application is no longer awaiting review.";
            await LoadApplicationsAsync();
            return Page();
        }

        var approver = await _context.SystemUsers
            .FirstOrDefaultAsync(u => u.SystemUserId == userId.Value);

        if (approver == null)
            return RedirectToPage("/Account/Login");

        var now = DateTime.UtcNow;
        var reason = ReasonText?.Trim() ?? string.Empty;
        if (application.BusinessProfile == null || application.JourneyStage == null)
            {
                Message = "The application is missing its business profile or journey stage.";
                await LoadApplicationsAsync();
                return Page();
            }
        var review = new ApprovalReview
        {
            ApplicationId = application.ApplicationId,
            ApproverId = approver.SystemUserId,
            Decision = Decision,
            ReasonText = reason
        };

        application.UpdateStatus(Decision);

        var businessOwnerId = application.BusinessProfile.SystemUserId;

        var notification = new Notification
        {
            SystemUserId = businessOwnerId,
            Message = Decision switch
            {
                "Approved" =>
                    $"Your {application.JourneyStage.StageName} application has been marked as approved. Check the official institution's system for confirmation.",
                "Rejected" =>
                    $"Your {application.JourneyStage.StageName} application was marked as rejected. Reason: {reason}",
                _ =>
                    $"Your {application.JourneyStage.StageName} application requires action. Details: {reason}"
            },
            IsRead = false,
            CreatedAt = now
        };

        _context.ApprovalReviews.Add(review);
        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Decision '{Decision}' saved successfully.";

        return RedirectToPage("/Approver/Index");
    }

        private async Task LoadApplicationsAsync()
    {
        // Applications that still need a decision.
        SubmittedApplications = await _context.Applications
            .Include(a => a.BusinessProfile)
            .Include(a => a.JourneyStage)
            .Where(a => a.Status == "Submitted")
            .OrderBy(a => a.SubmissionDate)
            .ToListAsync();

        // Applications that have already been reviewed.
        ReviewedApplications = await _context.Applications
            .Include(a => a.BusinessProfile)
            .Include(a => a.JourneyStage)
            .Where(a => a.Status == "Approved"
                    || a.Status == "Rejected"
                    || a.Status == "Action required")
            .OrderByDescending(a => a.SubmissionDate)
            .ToListAsync();
    }
}
