using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Funding;

public class EnquiryModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public EnquiryModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public BusinessBuzz.Core.Entities.BusinessProfile? Profile
        { get; private set; }

    public List<FundingAssessment> Assessments { get; private set; }
        = new();

    public string? Message { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? InfoMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 1)
            return Forbid();

        await LoadPageAsync(userId);

        if (Profile == null)
            return RedirectToPage("/BusinessProfile/Create");

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 1)
            return Forbid();

        var profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(p => p.SystemUserId == userId);

        if (profile == null)
            return RedirectToPage("/BusinessProfile/Create");

        // Do not create duplicate pending enquiries for this business.
        var existingPending = await _context.FundingAssessments
            .AnyAsync(f =>
                f.BusinessProfileId == profile.BusinessProfileId &&
                f.Status == "Pending");

        if (existingPending)
        {
            InfoMessage =
                "Your business already has a pending funding enquiry. " +
                "The banker must review it before another can be submitted.";

            return RedirectToPage();
        }

        var assessment = new FundingAssessment
        {
            BusinessProfileId = profile.BusinessProfileId,
            BankUserId = null,
            Status = "Pending",
            SimulatedOutcome = string.Empty,
            Remarks = string.Empty,
            SubmittedAt = DateTime.UtcNow
        };

        _context.FundingAssessments.Add(assessment);

        // Find registered banker accounts (RoleId = 3).
        var bankerIds = await _context.SystemUsers
            .Where(u => u.RoleId == 3)
            .Select(u => u.SystemUserId)
            .ToListAsync();

        // Notify each banker about the new enquiry.
        foreach (var bankerId in bankerIds)
        {
            _context.Notifications.Add(new Notification
            {
                SystemUserId = bankerId,
                Message = $"A new funding enquiry has been submitted for " +
                        $"{profile.BusinessName}. It is awaiting your review.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        SuccessMessage =
            "Your funding enquiry has been submitted for banker review.";

        return RedirectToPage();
    }

    private async Task LoadPageAsync(int userId)
    {
        Profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(p => p.SystemUserId == userId);

        if (Profile == null)
            return;

        Assessments = await _context.FundingAssessments
            .Where(f => f.BusinessProfileId == Profile.BusinessProfileId)
            .OrderByDescending(f => f.SubmittedAt)
            .ToListAsync();
    }
}