namespace BusinessBuzz.Core.DTOs;

public class BuzzGuideContext
{
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string BusinessLocation { get; set; } = string.Empty;

    public bool EmploysPeople { get; set; }
    public bool SeeksFunding { get; set; }

    public List<JourneyStageContext> Journey { get; set; } = new();
}