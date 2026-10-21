using BusinessBuzz.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthenticationService _authenticationService;

    public LoginModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Email is required.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Password is required.";
            return Page();
        }

        var user = await _authenticationService.LoginAsync(
            Email.Trim(),
            Password);

        if (user == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        //return Page();
        HttpContext.Session.SetInt32("UserId", user.SystemUserId);
        HttpContext.Session.SetInt32("RoleId", user.RoleId);


        if (user.RoleId == 1)
        {
            return RedirectToPage("/Dashboard/Index");
        }

        if (user.RoleId == 2)
        {
            return RedirectToPage("/Approver/Index");
        }

        if (user.RoleId == 3)
        {
            // We'll create the Bank User dashboard later.
            return RedirectToPage("/Bank/Index");
        }

        HttpContext.Session.Clear();
        ErrorMessage = "Your account has an unrecognised role.";
        return Page();

    }
}