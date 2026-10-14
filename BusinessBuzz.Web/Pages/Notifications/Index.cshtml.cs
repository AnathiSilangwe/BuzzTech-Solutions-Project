using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Web.Pages.Notifications;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<Notification> Notifications { get; private set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Notifications = await _context.Notifications
            .Where(n => n.SystemUserId == userId.Value)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int notificationId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId &&
                n.SystemUserId == userId.Value);

        if (notification == null)
        {
            return NotFound();
        }

        if (!notification.IsRead)
        {
            notification.MarkAsRead();
            await _context.SaveChangesAsync();
            SuccessMessage = "Notification marked as read.";
        }

        return RedirectToPage();
    }
}