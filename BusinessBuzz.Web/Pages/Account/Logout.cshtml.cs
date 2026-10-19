using Microsoft.AspNetCore.Mvc;
using BusinessBuzz.Core.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages.Account;
public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Account/Login");
    }
}