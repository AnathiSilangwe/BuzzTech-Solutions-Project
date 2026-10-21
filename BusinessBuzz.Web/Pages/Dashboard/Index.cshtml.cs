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
    private readonly IBuzzGuideConversationService _buzzGuideConversationService;

    public IndexModel( BusinessBuzzDbContext context, IJourneyService journeyService, IBuzzGuideContextService buzzGuideContextService, IBuzzGuideService buzzGuideService, IBuzzGuideConversationService buzzGuideConversationService)
    {
        _context = context;
        _journeyService = journeyService;
        _buzzGuideContextService = buzzGuideContextService;
        _buzzGuideService = buzzGuideService;
        _buzzGuideConversationService = buzzGuideConversationService;
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
    public string? BuzzGuideQuestion { get; set; }
    public string? BuzzGuideResponse { get; private set; }

    public bool HasBusinessProfile { get; private set; }

    public List<BuzzGuideMessage> BuzzGuideConversation { get; private set; } = new();
    public List<JourneyStage> PersonalisedJourney { get; private set; } = new();
    public Dictionary<int, string> ApplicationStatuses {get; private set; } = new();
    public List<Notification> RecentNotifications { get; private set; } = new();
    public int UnreadNotificationCount {get; private set; }

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

        RecentNotifications = await _context.Notifications
            .Where(n => n.SystemUserId == UserId.Value)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        UnreadNotificationCount = await _context.Notifications
            .CountAsync(n => n.SystemUserId == UserId.Value && !n.IsRead);

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
             var applications = await _context.Applications
                .Where(a => a.BusinessProfileId == profile.BusinessProfileId)
                .ToListAsync();

            ApplicationStatuses = applications
                .GroupBy(a => a.JourneyStageId)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderByDescending(a => a.SubmissionDate)
                                .First().Status
                );
            BuzzGuideContext = await _buzzGuideContextService.GetContextAsync( profile.BusinessProfileId);
            /* if (BuzzGuideContext != null)
            {
                BuzzGuideResponse = await _buzzGuideService.GetResponseAsync( BuzzGuideContext, "Tell me about my business.");
            } */
            BuzzGuideConversation = await _buzzGuideConversationService.GetConversationAsync(profile.BusinessProfileId);
        }



        return Page();
    }
    
    public async Task<IActionResult> OnPostAskBuzzGuideAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Your session has expired. Please log in again."
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        if (string.IsNullOrWhiteSpace(BuzzGuideQuestion))
        {
            return BadRequest(new
            {
                success = false,
                message = "Please enter a question."
            });
        }

        var question = BuzzGuideQuestion.Trim();

        if (question.Length > 1000)
        {
            return BadRequest(new
            {
                success = false,
                message = "Your question must not exceed 1000 characters."
            });
        }

        var profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(b => b.SystemUserId == userId.Value);

        if (profile == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please create your business profile first."
            });
        }

        var businessProfileId = profile.BusinessProfileId;

        var buzzGuideContext =
            await _buzzGuideContextService.GetContextAsync(businessProfileId);

        if (buzzGuideContext == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Your business information could not be loaded."
            });
        }

        await _buzzGuideConversationService.AddMessageAsync(
            businessProfileId,
            "Entrepreneur",
            question);

        // Load the updated history, including the latest question.
        var conversation =
            await _buzzGuideConversationService
                .GetConversationAsync(businessProfileId);

        var answer = await _buzzGuideService.GetResponseAsync(
            buzzGuideContext,
            conversation,
            question);

        await _buzzGuideConversationService.AddMessageAsync(
            businessProfileId,
            "BuzzGuide",
            answer);

        return new JsonResult(new
        {
            success = true,
            question,
            answer
        });
    }

    
    public async Task<IActionResult> OnPostSubmitApplicationAsync(int journeyStageId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var roleId = HttpContext.Session.GetInt32("RoleId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (roleId != 1)
        {
            return Forbid();
        }

        var profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(b => b.SystemUserId == userId.Value);

        if (profile == null)
        {
            return RedirectToPage("/BusinessProfile/Create");
        }

        // Only CIPC Registration is actionable in this version.
        var stage = await _context.JourneyStages
            .FirstOrDefaultAsync(s =>
                s.JourneyStageId == journeyStageId &&
                s.StageName == "CIPC Registration");

        if (stage == null)
        {
            return NotFound();
        }

        var application = await _context.Applications
            .FirstOrDefaultAsync(a =>
                a.BusinessProfileId == profile.BusinessProfileId &&
                a.JourneyStageId == stage.JourneyStageId);

        if (application == null)
        {
            application = new Application
            {
                BusinessProfileId = profile.BusinessProfileId,
                JourneyStageId = stage.JourneyStageId
            };

            application.SubmitApplication();
            _context.Applications.Add(application);
        }
        else if (application.Status == "Rejected")
        {
            // Allow resubmission after rejection.
            application.SubmitApplication();
        }
        else
        {
            // Prevent duplicate notifications for an already submitted
            // or otherwise non-resubmittable application.
            return RedirectToPage("/Dashboard/Index");
        }

        // Find all registered approver accounts.
        var approverIds = await _context.SystemUsers
            .Where(u => u.RoleId == 2)
            .Select(u => u.SystemUserId)
            .ToListAsync();

        // Create an unread notification for each approver.
        foreach (var approverId in approverIds)
        {
            _context.Notifications.Add(new Notification
            {
                SystemUserId = approverId,
                Message = "A CIPC Registration application has been submitted and is awaiting review.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/Dashboard/Index");
    }

}