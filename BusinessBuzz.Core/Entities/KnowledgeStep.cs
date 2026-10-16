
using BusinessBuzz.Core.Entitites;

namespace BusinessBuss.Core.Entities;

public class KnowledgeStep
{
    public int KnowledgeStepId {get; set;}
    public int KnowledgeArticleId{get; set;}
    public int StepNumber {get; set;}
    public string Instruction {get; set; } = string.Empty;
    public KnowledgeArticle KnowledgeArticles {get; set;} = null!;
}