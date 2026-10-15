
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.BuzzGuide;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;
    private readonly IBuzzGuideContextService _contextService;
    private readonly IBuzzGuideService _buzzGuideService;
    private readonly IBuzzGuideConversationService _conversationService;

    public IndexModel(
        BusinessBuzzDbContext context,
        IBuzzGuideContextService contextService,
        IBuzzGuideService buzzGuideService,
        IBuzzGuideConversationService conversationService)
    {
        _context = context;
        _contextService = contextService;
        _buzzGuideService = buzzGuideService;
        _conversationService = conversationService;
    }

    public string BusinessName { get; private set; } = "your business";

    public List<BuzzGuideMessage> Conversation { get; private set; } = new();

    [BindProperty]
    public string Question { get; set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    private async Task<int?> GetBusinessProfileIdAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return null;

        var profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(p => p.SystemUserId == userId.Value);

        if (profile == null)
            return null;

        BusinessName = profile.BusinessName;
        return profile.BusinessProfileId;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        var profileId = await GetBusinessProfileIdAsync();

        if (profileId == null)
            return RedirectToPage("/BusinessProfile/Create");

        Conversation = await _conversationService
            .GetConversationAsync(profileId.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostAskAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");

        var profileId = await GetBusinessProfileIdAsync();

        if (profileId == null)
            return RedirectToPage("/BusinessProfile/Create");

        if (string.IsNullOrWhiteSpace(Question))
        {
            ErrorMessage = "Please enter a question.";
            Conversation = await _conversationService
                .GetConversationAsync(profileId.Value);
            return Page();
        }

        var context = await _contextService.GetContextAsync(profileId.Value);

        if (context == null)
        {
            ErrorMessage = "Your business information could not be loaded. Please check your profile.";
            Conversation = await _conversationService
                .GetConversationAsync(profileId.Value);
            return Page();
        }

        await _conversationService.AddMessageAsync(
            profileId.Value, "Entrepreneur", Question.Trim());

        // Reload history so the latest question is included.
        Conversation = await _conversationService
            .GetConversationAsync(profileId.Value);

        var answer = await _buzzGuideService.GetResponseAsync(
            context, Conversation, Question.Trim());

        await _conversationService.AddMessageAsync(
            profileId.Value, "BuzzGuide", answer);

        return RedirectToPage("/BuzzGuide/Index");
    }
}
