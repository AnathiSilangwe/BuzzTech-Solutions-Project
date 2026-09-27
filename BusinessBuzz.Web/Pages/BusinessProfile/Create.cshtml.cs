using BusinessProfileEntity = BusinessBuzz.Core.Entities.BusinessProfile;
using BusinessTypeEntity = BusinessBuzz.Core.Entities.BusinessType;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.BusinessProfile;

public class CreateModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public CreateModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public BusinessProfileEntity BusinessProfile { get; set; } = new();

    public SelectList BusinessTypes { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var existingProfile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(b => b.SystemUserId == userId.Value);

        if (existingProfile != null)
        {
            return RedirectToPage("/Dashboard/Index");
        }

        await LoadBusinessTypesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (string.IsNullOrWhiteSpace(BusinessProfile.BusinessName))
        {
            ErrorMessage = "Business name is required.";
            await LoadBusinessTypesAsync();
            return Page();
        }

        if (BusinessProfile.BusinessTypeId <= 0)
        {
            ErrorMessage = "Please select a business type.";
            await LoadBusinessTypesAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(BusinessProfile.Industry))
        {
            ErrorMessage = "Industry is required.";
            await LoadBusinessTypesAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(BusinessProfile.BusinessLocation))
        {
            ErrorMessage = "Business location is required.";
            await LoadBusinessTypesAsync();
            return Page();
        }

        var existingProfile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(b => b.SystemUserId == userId.Value);

        if (existingProfile != null)
        {
            return RedirectToPage("/Dashboard/Index");
        }

        var businessTypeExists = await _context.BusinessTypes
            .AnyAsync(t => t.BusinessTypeId == BusinessProfile.BusinessTypeId);

        if (!businessTypeExists)
        {
            ErrorMessage = "The selected business type is invalid.";
            await LoadBusinessTypesAsync();
            return Page();
        }

        BusinessProfile.SystemUserId = userId.Value;
        BusinessProfile.IsConfirmed = true;

        _context.BusinessProfiles.Add(BusinessProfile);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Dashboard/Index");
    }

    private async Task LoadBusinessTypesAsync()
    {
        var businessTypes = await _context.BusinessTypes
            .OrderBy(t => t.TypeName)
            .ToListAsync();

        BusinessTypes = new SelectList(
            businessTypes,
            nameof(BusinessTypeEntity.BusinessTypeId),
            nameof(BusinessTypeEntity.TypeName));
    }
}