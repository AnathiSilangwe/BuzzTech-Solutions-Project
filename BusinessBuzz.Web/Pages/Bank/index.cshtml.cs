using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Bank;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<FundingAssessment> PendingAssessments { get; private set; } = new();

    public List<FundingAssessment> CompletedAssessments { get; private set; } = new();

    public int UnreadNotifications { get; private set; }

    private bool IsBankUser()
    {
        return HttpContext.Session.GetInt32("RoleId") == 3;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        if (!IsBankUser())
            return Forbid();

        PendingAssessments = await _context.FundingAssessments
            .Include(f => f.BusinessProfile)
                .ThenInclude(b => b.BusinessType)
            .Where(f => f.Status == "Pending")
            .OrderBy(f => f.SubmittedAt)
            .ToListAsync();

        CompletedAssessments = await _context.FundingAssessments
            .Include(f => f.BusinessProfile)
                .ThenInclude(b => b.BusinessType)
            .Include(f => f.BankUser)
            .Where(f => f.Status == "Completed")
            .OrderByDescending(f => f.AssessedAt)
            .ToListAsync();

        UnreadNotifications = await _context.Notifications
            .CountAsync(n => n.SystemUserId == userId.Value && !n.IsRead);

        return Page();
    }
}