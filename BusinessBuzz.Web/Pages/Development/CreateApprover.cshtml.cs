using BusinessBuzz.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages.Development;

public class CreateApproverModel : PageModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public CreateApproverModel(
        IAuthenticationService authenticationService,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
        _environment = environment;
    }
    [BindProperty]
    public string ApproverName {get; set;} = string.Empty;

    [BindProperty]
    public string SetupKey { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? Message { get; private set; }

    public IActionResult OnGet()
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        var expectedKey = _configuration["DevelopmentSetup:ApproverSetupKey"];

        if (string.IsNullOrWhiteSpace(expectedKey) ||
            !string.Equals(SetupKey, expectedKey, StringComparison.Ordinal))
        {
            Message = "Invalid setup key.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) ||
            Password.Length < 8 || string.IsNullOrWhiteSpace(ApproverName))
        {
            Message = "Enter an email and a password of at least 8 characters.";
            return Page();
        }

        // Role 2 = Approver.
        var user = await _authenticationService.RegisterAsync(
            ApproverName,
            Email.Trim(),
            Password,
            2);

        if (user == null)
        {
            Message = "An account with that email already exists.";
            return Page();
        }

        Message = "Approver account created. You can now sign in using the normal Login page.";
        Email = string.Empty;
        Password = string.Empty;
        SetupKey = string.Empty;

        return Page();
    }
}
