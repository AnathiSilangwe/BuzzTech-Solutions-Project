using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Services;

public interface IJourneyService
{
    Task<List<JourneyStage>> GetPersonalisedJourneyAsync(
        int businessProfileId);
}