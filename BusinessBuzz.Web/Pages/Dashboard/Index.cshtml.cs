using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages.Dashboard;

public class IndexModel : PageModel
{
    public int? UserId { get; private set; }
    public int? RoleId { get; private set; }

    public IActionResult OnGet()
    {
        UserId = HttpContext.Session.GetInt32("UserId");
        RoleId = HttpContext.Session.GetInt32("RoleId");

        if (UserId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        return Page();
    }
}