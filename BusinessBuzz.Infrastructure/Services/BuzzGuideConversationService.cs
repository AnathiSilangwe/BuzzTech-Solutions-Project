using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideConversationService : IBuzzGuideConversationService
{
    private readonly BusinessBuzzDbContext _context;

    public BuzzGuideConversationService(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public async Task<List<BuzzGuideMessage>> GetConversationAsync(
        int businessProfileId)
    {
        return await _context.BuzzGuideMessages
            .Where(m => m.BusinessProfileId == businessProfileId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<BuzzGuideMessage> AddMessageAsync(
        int businessProfileId,
        string role,
        string message)
    {
        var buzzGuideMessage = new BuzzGuideMessage
        {
            BusinessProfileId = businessProfileId,
            Role = role,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        _context.BuzzGuideMessages.Add(buzzGuideMessage);

        await _context.SaveChangesAsync();

        return buzzGuideMessage;
    }
}