using BusinessBuzz.Core.DTOs;

namespace BusinessBuzz.Core.Services;

public interface IBuzzGuideContextService
{
    Task<BuzzGuideContext?> GetContextAsync(int businessProfileId);
}