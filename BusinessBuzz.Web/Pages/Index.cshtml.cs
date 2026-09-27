using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BusinessBuzz.Web.Pages;

public class IndexModel : PageModel
{
    public string PageTitle { get; private set; } = "Business Buzz";

    public string Introduction { get; private set; } =
        "Your guide to turning a business idea into a real business.";

    public void OnGet()
    {
        // The home page currently does not require any database operations.
        // Business logic can be added here later when needed.
    }
}