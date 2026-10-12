using BusinessBuzz.Core.Entities;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Web.Pages.Documents;

public class IndexModel : PageModel
{
    private readonly BusinessBuzzDbContext _context;
    public IndexModel(BusinessBuzzDbContext context)
    {
        _context = context;
    }
    public List <Document> Documents {get; private set; } = new ();
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

    
        BusinessBuzz.Core.Entities.BusinessProfile? businessProfile = await _context.BusinessProfiles.FirstOrDefaultAsync(b => b.SystemUserId == userId.Value);
        
        if(businessProfile == null){
            return RedirectToPage("/BusinessProfile/Create");
        } 
        Documents = await _context.Documents
            .Include(d => d.DocumentType)
            .Where(d => d.BusinessProfileId == businessProfile.BusinessProfileId)
            .OrderByDescending(d =>d.UploadedAt)
            .ToListAsync();
        
        return Page();
    }

}