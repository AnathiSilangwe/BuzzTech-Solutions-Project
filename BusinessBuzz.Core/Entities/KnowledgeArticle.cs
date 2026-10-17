
using BusinessBuzz.Core.Entities;

namespace BusinessBuzz.Core.Entities;
public class KnowledgeArticle
{
    public int KnowledgeArticleId{get; set;}
    public string Title {get; set;} = string.Empty;
    public string Topic {get; set;} = string.Empty;
    public string Summary {get; set;} = string.Empty;
    public string Content {get; set;} = string.Empty;
    public string Keywords {get; set;} = string.Empty;
    public string SourceName {get; set;} = string.Empty;
    public string SourceUrl {get; set;} = string.Empty;
    public DateTime LastVerifiedAt {get; set;}
    public bool IsActive { get; set; } = true;
    public ICollection<KnowledgeStep> Steps {get; set; } = new List<KnowledgeStep>();

}