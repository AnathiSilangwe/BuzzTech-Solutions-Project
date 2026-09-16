namespace BusinessBuzz.Core.Entities;

public class JourneyStage
{
    public int JourneyStageId {get;set;}
    public string StageName {get; set;} = string.Empty;
    public string InstitutionName {get; set;} = string.Empty;
    public string StageDescription {get; set;} = string.Empty;
    public int StageOrder { get; set; }
    public bool IsRequired { get; set; }
    public string StageType { get; set; } = string.Empty;
    
    //Navigation property
    public ICollection<Application> Applications { get; set; } = new List<Application>();

    public string GetStageDetails()
    {
        return StageDescription;
    }
}