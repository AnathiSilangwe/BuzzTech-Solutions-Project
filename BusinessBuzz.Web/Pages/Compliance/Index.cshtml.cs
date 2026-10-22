using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Compliance;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<ComplianceRecord> Records { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var roleId = HttpContext.Session.GetInt32("RoleId");

        if (userId == null)
            return RedirectToPage("/Account/Login");

        if (roleId != 1)
            return Forbid();

        Records = await _context.ComplianceRecords
            .Include(c => c.BusinessProfile)
            .Where(c => c.BusinessProfile.SystemUserId == userId.Value)
            .OrderByDescending(c => c.ExpiryDate)
            .ToListAsync();

        return Page();
    }
}
