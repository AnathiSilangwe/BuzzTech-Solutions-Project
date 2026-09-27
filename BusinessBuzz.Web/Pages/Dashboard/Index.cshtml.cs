using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Dashboard;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public int? UserId { get; private set; }
    public int? RoleId { get; private set; }

    public string? UserEmail { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        UserId = HttpContext.Session.GetInt32("UserId");
        RoleId = HttpContext.Session.GetInt32("RoleId");

        if (UserId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await _context.SystemUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.SystemUserId == UserId.Value);

        if (user == null)
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Account/Login");
        }

        UserEmail = user.Email;

        return Page();
    }
}