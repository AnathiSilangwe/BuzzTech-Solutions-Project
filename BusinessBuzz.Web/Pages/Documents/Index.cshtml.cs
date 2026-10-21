using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Documents;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;

    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public List<Document> Documents { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var businessProfile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(
                b => b.SystemUserId == userId.Value);

        if (businessProfile == null)
        {
            return RedirectToPage("/BusinessProfile/Create");
        }

        Documents = await _context.Documents
            .Include(d => d.DocumentType)
            .Where(d =>
                d.BusinessProfileId ==
                businessProfile.BusinessProfileId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnGetDownloadAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // Only retrieve a document belonging to this entrepreneur.
        var document = await _context.Documents
            .Where(d =>
                d.DocumentId == id &&
                _context.BusinessProfiles.Any(b =>
                    b.BusinessProfileId == d.BusinessProfileId &&
                    b.SystemUserId == userId.Value))
            .Select(d => new
            {
                d.FilePath
            })
            .FirstOrDefaultAsync();

        // Do not reveal whether another entrepreneur's
        // document exists.
        if (document == null ||
            string.IsNullOrWhiteSpace(document.FilePath))
        {
            return NotFound();
        }

        var filePath = Path.GetFullPath(document.FilePath);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("The document file could not be found.");
        }

        var contentTypeProvider = new FileExtensionContentTypeProvider();

        if (!contentTypeProvider.TryGetContentType(
                filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var downloadName = Path.GetFileName(filePath);

        return PhysicalFile(
            filePath,
            contentType,
            downloadName);
    }
}
