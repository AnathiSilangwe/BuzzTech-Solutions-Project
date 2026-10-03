using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Services;

public interface IBuzzGuideConversationService
{
    Task<List<BuzzGuideMessage>> GetConversationAsync(int businessProfileId);

    Task<BuzzGuideMessage> AddMessageAsync(
        int businessProfileId,
        string role,
        string message);
}