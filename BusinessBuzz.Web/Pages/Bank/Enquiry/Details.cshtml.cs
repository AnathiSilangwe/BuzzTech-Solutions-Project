using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Bank.Enquiry;

public class DetailsModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public DetailsModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public FundingAssessment? Assessment { get; private set; }

    public List<ReadinessTask> ReadinessTasks { get; private set; } = new();

    public List<ComplianceRecord> ComplianceRecords { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var roleId = HttpContext.Session.GetInt32("RoleId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (roleId != 3)
        {
            return Forbid();
        }

        Assessment = await _context.FundingAssessments
            .Include(f => f.BusinessProfile)
                .ThenInclude(b => b.BusinessType)
            .FirstOrDefaultAsync(f => f.FundingAssessmentId == id);

        if (Assessment == null)
        {
            return NotFound();
        }

        ReadinessTasks = await _context.ReadinessTasks
            .Where(t => t.BusinessProfileId == Assessment.BusinessProfileId)
            .OrderBy(t => t.TaskName)
            .ToListAsync();

        ComplianceRecords = await _context.ComplianceRecords
            .Where(c => c.BusinessProfileId == Assessment.BusinessProfileId)
            .OrderBy(c => c.ComplianceType)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAssessAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var roleId = HttpContext.Session.GetInt32("RoleId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (roleId != 3)
        {
            return Forbid();
        }

        var assessment = await _context.FundingAssessments
            .Include(f => f.BusinessProfile)
            .FirstOrDefaultAsync(f => f.FundingAssessmentId == id);
        if (assessment == null)
        {
            return NotFound();
        }

        if (assessment.Status != "Pending")
        {
            TempData["ErrorMessage"] =
                "This enquiry has already been assessed.";

            return RedirectToPage(
                "/Bank/Enquiry/Details",
                new { id });
        }

        var allowedOutcomes = new[]
        {
            "Approved",
            "Declined",
            "More information required"
        };

        var outcome = Request.Form["SimulatedOutcome"].ToString();
        var remarks = Request.Form["Remarks"].ToString().Trim();

        if (!allowedOutcomes.Contains(outcome))
        {
            ModelState.AddModelError(
                "SimulatedOutcome",
                "Please select a valid assessment outcome.");

            await LoadEnquiryAsync(id);
            return Page();
        }

        if (string.IsNullOrWhiteSpace(remarks) || remarks.Length > 2000)
        {
            ModelState.AddModelError(
                "Remarks",
                "Remarks are required and must not exceed 2000 characters.");

            await LoadEnquiryAsync(id);
            return Page();
        }


        assessment.SimulatedOutcome = outcome;
        assessment.Remarks = remarks;
        assessment.Status = "Completed";
        assessment.BankUserId = userId.Value;
        assessment.AssessedAt = DateTime.UtcNow;

        var businessName = assessment.BusinessProfile?.BusinessName;

        if (string.IsNullOrWhiteSpace(businessName))
        {
            businessName = "your business";
        }

        var notification = new Notification
        {
            SystemUserId = assessment.BusinessProfile!.SystemUserId,
            Message = $"The simulated funding assessment for {businessName} " +
                    $"has been completed. Outcome: {outcome}. " +
                    "Sign in to Business Buzz to view the assessment remarks.",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();


        TempData["SuccessMessage"] =
            "Funding assessment recorded successfully.";

        return RedirectToPage(
            "/Bank/Enquiry/Details",
            new { id = assessment.FundingAssessmentId });
    }

    private async Task LoadEnquiryAsync(int id)
    {
        Assessment = await _context.FundingAssessments
            .Include(f => f.BusinessProfile)
                .ThenInclude(b => b.BusinessType)
            .FirstOrDefaultAsync(f => f.FundingAssessmentId == id);

        if (Assessment != null)
        {
            ReadinessTasks = await _context.ReadinessTasks
                .Where(t => t.BusinessProfileId == Assessment.BusinessProfileId)
                .OrderBy(t => t.TaskName)
                .ToListAsync();

            ComplianceRecords = await _context.ComplianceRecords
                .Where(c => c.BusinessProfileId == Assessment.BusinessProfileId)
                .OrderBy(c => c.ComplianceType)
                .ToListAsync();
        }
    }
}
