namespace BusinessBuzz.Core.Entities;

public class BuzzGuideMessage
{
    public int BuzzGuideMessageId { get; set; }

    public int BusinessProfileId { get; set; }

    public string Role { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public BusinessProfile BusinessProfile { get; set; } = null!;
}