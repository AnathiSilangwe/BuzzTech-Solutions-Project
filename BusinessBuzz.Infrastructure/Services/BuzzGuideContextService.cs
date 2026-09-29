using BusinessBuzz.Core.DTOs;
using BusinessBuzz.Core.Services;
using BusinessBuzz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessBuzz.Infrastructure.Services;

public class BuzzGuideContextService : IBuzzGuideContextService
{
    private readonly BusinessBuzzDbContext _context;
    private readonly IJourneyService _journeyService;

    public BuzzGuideContextService(
        BusinessBuzzDbContext context,
        IJourneyService journeyService)
    {
        _context = context;
        _journeyService = journeyService;
    }

    public async Task<BuzzGuideContext?> GetContextAsync(
        int businessProfileId)
    {
        var businessProfile = await _context.BusinessProfiles
            .Include(b => b.BusinessType)
            .FirstOrDefaultAsync(
                b => b.BusinessProfileId == businessProfileId);

        if (businessProfile == null)
        {
            return null;
        }

        var journey = await _journeyService
            .GetPersonalisedJourneyAsync(businessProfileId);

        var context = new BuzzGuideContext
        {
            BusinessName = businessProfile.BusinessName,
            BusinessType = businessProfile.BusinessType.TypeName,
            Industry = businessProfile.Industry,
            BusinessLocation = businessProfile.BusinessLocation,
            EmploysPeople = businessProfile.EmploysPeople,
            SeeksFunding = businessProfile.SeeksFunding
        };

        foreach (var stage in journey)
        {
            context.Journey.Add(new JourneyStageContext
            {
                StageName = stage.StageName,
                InstitutionName = stage.InstitutionName,
                StageDescription = stage.StageDescription,
                StageOrder = stage.StageOrder,
                IsRequired = stage.IsRequired,
                StageType = stage.StageType
            });
        }

        return context;
    }
}