using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Services;

public interface IBuzzGuideService
{
    Task<string> GetResponseAsync(
    BuzzGuideContext context,
    List<BuzzGuideMessage> conversation,
    string question);
}