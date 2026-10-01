using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Core.DTOs;

namespace BusinessBuzz.Web.Pages.Dashboard;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;
    private readonly IJourneyService _journeyService;
    private readonly IBuzzGuideContextService _buzzGuideContextService;
    private readonly IBuzzGuideService _buzzGuideService;

    public IndexModel( BusinessBuzzDbContext context, IJourneyService journeyService, IBuzzGuideContextService buzzGuideContextService, IBuzzGuideService buzzGuideService)
    {
        _context = context;
        _journeyService = journeyService;
        _buzzGuideContextService = buzzGuideContextService;
        _buzzGuideService = buzzGuideService;
    }

    public int? UserId { get; private set; }
    public int? RoleId { get; private set; }

    public string? UserEmail { get; private set; }

    public string? BusinessName { get; private set; }
    public string? BusinessType { get; private set; }
    public string? Industry { get; private set; }
    public string? BusinessLocation { get; private set; }
    public bool? EmploysPeople { get; private set; }
    public bool? IsProfileConfirmed { get; private set; }
    public BuzzGuideContext? BuzzGuideContext { get; private set; }
    public string? BuzzGuideResponse { get; private set; }

    public bool HasBusinessProfile { get; private set; }

    public List<JourneyStage> PersonalisedJourney { get; private set; } = new();

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

        var profile = await _context.BusinessProfiles
            .Include(b => b.BusinessType)
            .FirstOrDefaultAsync(b => b.SystemUserId == UserId.Value);

        if (profile != null)
        {
            HasBusinessProfile = true;

            BusinessName = profile.BusinessName;
            BusinessType = profile.BusinessType.TypeName;
            Industry = profile.Industry;
            BusinessLocation = profile.BusinessLocation;
            EmploysPeople = profile.EmploysPeople;
            IsProfileConfirmed = profile.IsConfirmed;

            PersonalisedJourney = await _journeyService.GetPersonalisedJourneyAsync(profile.BusinessProfileId);
            BuzzGuideContext = await _buzzGuideContextService.GetContextAsync( profile.BusinessProfileId);
            if (BuzzGuideContext != null)
            {
                BuzzGuideResponse = await _buzzGuideService.GetResponseAsync( BuzzGuideContext, "Tell me about my business.");
            }
        }



        return Page();
    }
}