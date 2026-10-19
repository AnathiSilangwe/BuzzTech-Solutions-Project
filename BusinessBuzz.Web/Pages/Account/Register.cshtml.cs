using BusinessBuzz.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthenticationService _authenticationService;

    public RegisterModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    [BindProperty]
    public string Name { get; set; } = string.Empty;
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

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

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        // Public registration creates Entrepreneur accounts only.
        const int entrepreneurRoleId = 1;

        var user = await _authenticationService.RegisterAsync(
            Name.Trim(),
            Email.Trim(),
            Password,
            entrepreneurRoleId);

        if (user == null)
        {
            ErrorMessage = "An account with this email already exists.";
            return Page();
        }

        return RedirectToPage("/Account/Login");
    }
}