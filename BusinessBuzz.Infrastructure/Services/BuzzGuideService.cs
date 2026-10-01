using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Services;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideService : IBuzzGuideService
{
    public Task<string> GetResponseAsync(
        BuzzGuideContext context,
        string question)
    {
        var response =
            $"Hello! I am BuzzGuide for {context.BusinessName}. " +
            $"Your business is a {context.BusinessType} in the " +
            $"{context.Industry} industry. " +
            $"You currently have {context.Journey.Count} journey stages.";

        return Task.FromResult(response);
    }
}