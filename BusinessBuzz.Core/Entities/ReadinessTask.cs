namespace BusinessBuzz.Core.Entities;

public class ReadinessTask
{
    public int ReadinessTaskId { get; set; }
    public int BusinessProfileId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    // Navigation property
    public BusinessProfile BusinessProfile { get; set; } = null!;
    public void ToggleTaskStatus()
    {
        IsComplete = !IsComplete;
    }
}