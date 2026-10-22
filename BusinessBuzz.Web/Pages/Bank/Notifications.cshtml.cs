
using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Bank;

public class NotificationsModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public NotificationsModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<Notification> Notifications { get; private set; } = new();

    public int UnreadCount => Notifications.Count(n => !n.IsRead);

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 3)
            return Forbid();

        Notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.SystemUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostMarkAsReadAsync(int notificationId)
    {
        if (HttpContext.Session.GetInt32("UserId") is not int userId)
            return RedirectToPage("/Account/Login");

        if (HttpContext.Session.GetInt32("RoleId") != 3)
            return Forbid();

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId &&
                n.SystemUserId == userId);

        if (notification == null)
            return NotFound();

        notification.MarkAsRead();

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}
