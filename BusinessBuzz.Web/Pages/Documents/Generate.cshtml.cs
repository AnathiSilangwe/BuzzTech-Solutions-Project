using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Documents;

public class GenerateModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;
    private readonly IDigiDocsService _digiDocsService;

    public GenerateModel(
        BusinessBuzzDbContext context,
        IDigiDocsService digiDocsService)
    {
        _context = context;
        _digiDocsService = digiDocsService;
    }

    [BindProperty]
    public string IdNumber { get; set; } = string.Empty;

    public BusinessBuzz.Core.Entities.BusinessProfile? BusinessProfile { get; private set; }

    public string BusinessName { get; private set; } = string.Empty;

    public string BusinessTypeName { get; private set; } = string.Empty;

    public Document? GeneratedDocument { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        BusinessProfile = await _context.BusinessProfiles
            .Include(b => b.BusinessType)
            .FirstOrDefaultAsync(
                b => b.SystemUserId == userId.Value);

        if (BusinessProfile == null)
        {
            return RedirectToPage("/BusinessProfile/Create");
        }

        BusinessName = BusinessProfile.BusinessName;
        BusinessTypeName = BusinessProfile.BusinessType.TypeName;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        BusinessProfile = await _context.BusinessProfiles
            .Include(b => b.BusinessType)
            .FirstOrDefaultAsync(
                b => b.SystemUserId == userId.Value);

        if (BusinessProfile == null)
        {
            return RedirectToPage("/BusinessProfile/Create");
        }

        BusinessName = BusinessProfile.BusinessName;
        BusinessTypeName = BusinessProfile.BusinessType.TypeName;

        if (string.IsNullOrWhiteSpace(IdNumber))
        {
            ModelState.AddModelError(
                nameof(IdNumber),
                "Please enter your ID number.");
        }
        else if (IdNumber.Length != 13 ||
                 !IdNumber.All(char.IsDigit))
        {
            ModelState.AddModelError(
                nameof(IdNumber),
                "Please enter a valid 13-digit ID number.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        GeneratedDocument =
            await _digiDocsService
                .GenerateBusinessInformationDocumentAsync(
                    BusinessProfile.BusinessProfileId,
                    IdNumber);

        if (GeneratedDocument == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "The document could not be generated.");

            return Page();
        }

        return Page();
    }
}