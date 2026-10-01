using BusinessBuzz.Core.DTOs;

namespace BusinessBuzz.Core.Services;

public interface IBuzzGuideService
{
    Task<string> GetResponseAsync(
        BuzzGuideContext context,
        string question);
}