namespace BusinessBuzz.Core.DTOs;

public class JourneyStageContext
{
    public string StageName { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string StageDescription { get; set; } = string.Empty;
    public int StageOrder { get; set; }
    public bool IsRequired { get; set; }
    public string StageType { get; set; } = string.Empty;

    public string? ApplicationStatus { get; set; }
}