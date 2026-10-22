
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Funding;

public class ReadinessModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public ReadinessModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public BusinessBuzz.Core.Entities.BusinessProfile? Profile
{
    get;
    private set;
}

    public List<ReadinessTask> Tasks { get; private set; } = new();

    public int CompletedCount => Tasks.Count(t => t.IsComplete);

    public int Progress => Tasks.Count == 0
        ? 0
        : (int)Math.Round(CompletedCount * 100.0 / Tasks.Count);

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 1)
            return Forbid();

        Profile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(p => p.SystemUserId == userId);

        if (Profile == null)
            return RedirectToPage("/BusinessProfile/Create");

        await EnsureTasksAsync(Profile.BusinessProfileId);
        await LoadTasksAsync(Profile.BusinessProfileId);

        return Page();
    }

    public async Task<IActionResult> OnPostToggleAsync(int taskId)
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 1)
            return Forbid();

        var profileId = await _context.BusinessProfiles
            .Where(p => p.SystemUserId == userId)
            .Select(p => (int?)p.BusinessProfileId)
            .FirstOrDefaultAsync();

        if (profileId == null)
            return RedirectToPage("/BusinessProfile/Create");

        var task = await _context.ReadinessTasks
            .FirstOrDefaultAsync(t =>
                t.ReadinessTaskId == taskId &&
                t.BusinessProfileId == profileId.Value);

        if (task == null)
            return NotFound();

        task.ToggleTaskStatus();

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task EnsureTasksAsync(int profileId)
    {
        if (await _context.ReadinessTasks
            .AnyAsync(t => t.BusinessProfileId == profileId))
            return;

        var taskNames = new[]
        {
            "Complete my business profile",
            "Review my CIPC registration status",
            "Review my SARS and eFiling readiness",
            "Prepare supporting business documents",
            "Organise my business plan and goals",
            "Review my funding needs and financial information"
        };

        foreach (var name in taskNames)
        {
            _context.ReadinessTasks.Add(new ReadinessTask
            {
                BusinessProfileId = profileId,
                TaskName = name,
                IsComplete = false
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task LoadTasksAsync(int profileId)
    {
        Tasks = await _context.ReadinessTasks
            .Where(t => t.BusinessProfileId == profileId)
            .OrderBy(t => t.ReadinessTaskId)
            .ToListAsync();
    }
}
