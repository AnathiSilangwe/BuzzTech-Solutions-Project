using BusinessBuzz.Core.Entities;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Services;

public class JourneyService : IJourneyService
{
    private readonly BusinessBuzzDbContext _context;

    public JourneyService(BusinessBuzzDbContext context)
    {
        _context = context;
    }

    public async Task<List<JourneyStage>> GetPersonalisedJourneyAsync(
        int businessProfileId)
    {
        var businessProfile = await _context.BusinessProfiles
            .FirstOrDefaultAsync(b => b.BusinessProfileId == businessProfileId);

        if (businessProfile == null)
        {
            return new List<JourneyStage>();
        }

        var stages = await _context.JourneyStages
            .OrderBy(s => s.StageOrder)
            .ToListAsync();

        var personalisedJourney = new List<JourneyStage>();

        foreach (var stage in stages)
        {
            if (stage.StageName == "Business Profile")
            {
                personalisedJourney.Add(stage);
            }
            else if (stage.StageName == "CIPC Registration")
            {
                personalisedJourney.Add(stage);
            }
            else if (stage.StageName == "SARS Compliance")
            {
                personalisedJourney.Add(stage);
            }
            else if (stage.StageName == "UIF Compliance"
                     && businessProfile.EmploysPeople)
            {
                personalisedJourney.Add(stage);
            }
            else if (stage.StageName == "Business Readiness")
            {
                personalisedJourney.Add(stage);
            }
            else if (stage.StageName == "Funding Readiness"
                     && businessProfile.SeeksFunding)
            {
                personalisedJourney.Add(stage);
            }
        }

        return personalisedJourney;
    }
}