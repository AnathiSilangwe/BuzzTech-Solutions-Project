
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

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var roleId = HttpContext.Session.GetInt32("RoleId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // Only institutional approvers may access this page.
        if (roleId != 2)
        {
            return Forbid();
        }

        SubmittedApplications = await _context.Applications
            .Include(a => a.BusinessProfile)
            .Include(a => a.JourneyStage)
            .Where(a => a.Status == "Submitted")
            .OrderBy(a => a.SubmissionDate)
            .ToListAsync();

        return Page();
    }
}
